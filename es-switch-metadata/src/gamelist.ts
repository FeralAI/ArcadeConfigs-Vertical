import * as fs from 'fs';
import * as xml2js from 'xml2js';
import { find } from "lodash-es";
import { SearchSummary } from "./search.js";
import { on } from 'process';

export interface GamelistProvider {
    System: string;
    software: string;
    database: string;
    web: string;
}

export interface GamelistGame {
    '$': {
        id: string,
        source: string
    },
    path: string[];
    name: string[];
    desc: string[];
    developer: string[];
    publisher: string[];
    image: string[];
    players: string[];
    releasedate: string[];
    lang: string[];
    region: string[];
    genre: string[];
}

export interface GamelistRoot {
    provider: GamelistProvider;
    game: GamelistGame[];
}

export interface Gamelist {
    gameList: GamelistRoot;
}

// Merge game list objects. Assume old list contains all games and match and update against new list.
export function mergeGamelists(oldList: Gamelist, newList: Gamelist): Gamelist {
    const mergedList: Gamelist = {
        gameList: {
            provider: { ...newList.gameList.provider },
            game: [],
        },
    };

    oldList.gameList.game.forEach(oldGame => {
        const newGame = find(newList.gameList.game, { path: oldGame.path });
        let mergedGame: GamelistGame = { ...oldGame };
        if (newGame) {
            mergedGame.name = (newGame.name[0] !== '') ? newGame.name : mergedGame.name;
            mergedGame.desc = (newGame.desc[0] !== '') ? newGame.desc : mergedGame.desc;
            mergedGame.developer = (newGame.developer[0] !== '') ? newGame.developer : oldGame.developer;
            mergedGame.publisher = (newGame.publisher[0] !== '') ? newGame.publisher : mergedGame.publisher;
            mergedGame.image = (newGame.image[0] !== '') ? newGame.image : mergedGame.image;
            mergedGame.players = (newGame.players[0] !== '') ? newGame.players : mergedGame.players;
            mergedGame.releasedate = (newGame.releasedate[0] !== '') ? newGame.releasedate : mergedGame.releasedate;
            mergedGame.lang = (newGame.lang[0] !== '') ? newGame.lang : mergedGame.lang;
            mergedGame.region = (newGame.region[0] !== '') ? newGame.region : mergedGame.region;
            mergedGame.genre = (newGame.genre[0] !== '') ? newGame.genre : mergedGame.genre;
        }

        mergedList.gameList.game.push(mergedGame);
    });

    return mergedList;
}

export function createGamelist(matched: SearchSummary[]): Gamelist {
    // Build the new gamelist with some rules:
    // * Files are located in the same directory as the gamelist.xml file
    // * File names should begin the same as the title in control.nacp of the XCI,
    //   excluding invalid filesystem characters
    // * Images should use the exact name from control.nacp
    return {
        gameList: {
            provider: {
                System: 'Nintendo Switch',
                software: 'custom',
                database: 'Nintendo',
                web: 'https://github.com/favna/nintendo-switch-eshop',
            },
            game: matched.map(s => ({
                '$': { id: s.bestMatch.nsuid || '0', source: 'Nintendo' },
                path: [ `./${s.filename}` ],
                name: [ s.bestMatch.title || '' ],
                desc: [ s.bestMatch.description || '' ],
                developer: [ s.bestMatch.developer || '' ],
                publisher: [ s.bestMatch.publisher || '' ],
                image: [ `./media/images/${s.title}.jpg` ],
                players: [ s.bestMatch.players ],
                releasedate: [ s.bestMatch.releasedate || '' ],
                lang: [ s.lang || '' ],
                region: [ s.region || '' ],
                genre: [ s.genre || '' ],
            }))
        }
    };
}

export async function readGamelist(path: string): Promise<Gamelist> {
    const parser = new xml2js.Parser();
    const gamelistBuffer = await fs.promises.readFile(path);
    const gamelist = await parser.parseStringPromise(gamelistBuffer);
    gamelist.gameList.game.forEach((g: GamelistGame) => {
        if (g.name && !g.name[0] || g.name[0] === '\r\n    ') g.name = [ '' ];
        if (g.desc && !g.desc[0] || g.desc[0] === '\r\n    ') g.desc = [ '' ];
        if (g.developer && !g.developer[0] || g.developer[0] === '\r\n    ') g.developer = [ '' ];
        if (g.publisher && !g.publisher[0] || g.publisher[0] === '\r\n    ') g.publisher = [ '' ];
        if (g.releasedate && !g.releasedate[0] || g.releasedate[0] === '\r\n    ') g.releasedate = [ '' ];
    });

    return gamelist;
}

export async function saveGamelist(path: string, list: Gamelist): Promise<void> {
    const builder = new xml2js.Builder();
    const xml = builder.buildObject(list);
    await fs.promises.writeFile(path, xml);
}
