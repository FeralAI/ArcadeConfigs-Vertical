import * as fs from 'fs';
import * as xml2js from 'xml2js';
import { getGamesAmerica, getGamesEurope, getGamesJapan, getQueriedGamesAmerica } from 'nintendo-switch-eshop';
import { find } from 'lodash-es';

const dir = 'C:/ROMs/switch/';
const ext = '.xci';

/*
QueriedGameUS imporant properties:
title
softwareDeveloper
softwarePublisher
description (truncated)
genres[]
playerCount
releaseDateDisplay
developers[]
playerFilters
nsuid
*/

/*
Important properties:
* title
* description
* boxart
* horizontalHeaderImage
* publishers[]
* developers[]
* playerFilters[] ('1+', '2+')
* releaseDateDisplay (2017-11-22T08:00:00.000Z)
*/

/********** Define constants **********/
const excludedPhrases = ['arcade archives ', ' for nintendo switch'];
const replaceWithSpace = [' - '];

/********** Define functions **********/
const cleanTitle = (title) => {
    title = title.toLowerCase();
    replaceWithSpace.forEach(x => title = title.replace(x, ' '));
    excludedPhrases.forEach(x => title = title.replace(x, ''));
    title = title.replace('ⅱ', ' ii'); // Special handling for Strikers 1945 II
    title = title.replace('ⅲ', 'iii'); // Special handling for Strikers 1945 III
    title = title.replace(/[^\w ]/g, '').trim(); // Remove non-word chars

    return title;
}

const saveGamelist = async (list) => {
    var builder = new xml2js.Builder();
    var xml = builder.buildObject(list);
    await fs.promises.writeFile(dir + 'gamelist.xml', xml);
}

const createGamelist = (matched) => {
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
            game: matched.map(r => ({
                '$': { id: r.bestMatch.nsuid, source: 'Nintendo' },
                path: [ `./${r.filename}` ],
                name: [ r.bestMatch.title ],
                desc: [ r.bestMatch.description ],
                developer: [ r.bestMatch.softwareDeveloper || (r.bestMatch.developers || []).join(', ') || r.bestMatch.softwarePublisher ],
                publisher: [ r.bestMatch.softwarePublisher || r.bestMatch.softwareDeveloper || (r.bestMatch.developers || []).join(', ') ],
                image: [ `./media/images/${r.title}.jpg` ],
                players: [ r.bestMatch.playerCount.replace('+', '') ],
                releasedate: [ r.bestMatch.releaseDateDisplay ],
                lang: [ r.lang ],
                region: [ r.region ],
                genre: [ r.bestMatch.genres.join(', ') ],
            }))
        }
    };
}

// Merge game list objects. Assume old list contains all games and match and update against new list.
const mergeGamelists = (oldList, newList) => {
    let mergedList = {
        gameList: {
            provider: { ...newList.gameList.provider },
            game: [],
        },
    };

    oldList.gameList.game.forEach(g => {
        const newListGame = find(newList.gameList.game, { path: g.path });
        if (newListGame) {
            mergedList.gameList.game.push({
                ...g,
                ...newListGame,
                developer: g.developer || newListGame.developer,
            });
        }
        else {
            mergedList.gameList.game.push({ ...g });
        }
    });

    return mergedList;
};

/********** Do the thing **********/
const files = await fs.promises.readdir(dir);

// Query US API
const results = await Promise.all(files
    .filter(filename => filename.endsWith(ext))
    .map(async (filename) => {
        const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');

        let matches = await getQueriedGamesAmerica(`${title}`);
        matches = matches.filter(m => m.platformCode === 'NINTENDO_SWITCH');
        const searchTitle = cleanTitle(title);
        const exactMatches = matches.filter(m => cleanTitle(m.title) === searchTitle);

        const bestMatch = exactMatches.length === 1
            ? exactMatches[0]
            : matches.length === 1
                ? matches[0]
                : null;

        return {
            filename,
            title,
            searchTitle,
            matchCount: matches.length,
            exactMatchCount: exactMatches.length,
            bestMatch,
            allMatchesTitles: matches.map(m => m.title).join(', '),
            lang: 'en',
            region: 'us',
            // allMatches: JSON.stringify(matches),
        };
    }));

// console.log(results.filter(r => !r.bestMatch));

// Filter the results
const matched = results.filter(r => r.bestMatch);
const unmatched = results.filter(r => !r.bestMatch);

const parser = new xml2js.Parser();
const gamelistBuffer = await fs.promises.readFile(dir + 'gamelist.xml');
const oldList = await parser.parseStringPromise(gamelistBuffer);
const newList = createGamelist(matched);
const mergedList = mergeGamelists(oldList, newList);
await saveGamelist(mergedList);

// console.log(unmatched);
