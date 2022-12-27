import edge from 'edge-js';
import { filter, intersection, orderBy } from "lodash-es";
import { GameEU, GameJP, GameUS, getGamesBrazil, getGamesEurope, getGamesJapan, getQueriedGamesAmerica, QueriedGameUS } from "nintendo-switch-eshop";
import { Gamelist } from "./gamelist.js";
import { cleanTitle } from "./utils.js"

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

let dbBR: SearchGameBR[];
let dbEU: SearchGameEU[];
let dbJP: SearchGameJP[];
let dbUS: SearchGameUS[];

const matchQualityLimit = 0.75;
const excludedChars = ['\'', '-', '!', '?'];
const replaceWithSpaceChars = ['.'];
const excludedPhrases = [
    'arcade archives ',
    ' for nintendo switch',
];
const excludedWords = [
    '&',
    'a', 'an', 'and',
    'of',
    'the',
];

export interface SearchResult {
    nsuid: string;
    title: string;
    description: string;
    developer: string;
    publisher: string;
    players: string;
    releasedate: string;
    genre: string;
}

export interface SearchSummary {
    filename: string;
    title: string;
    searchTitle: string;
    matchCount: number;
    exactMatchCount: number;
    bestMatch: SearchResult,
    allMatchesTitles: string,
    lang: string,
    region: string,
    [key: string]: any;
}

export interface SearchGame {
    titleLower: string;
    matchQuality: number | undefined;
}

export interface SearchGameBR extends SearchGame, GameUS { }
export interface SearchGameUS extends SearchGame, GameUS { }
export interface SearchGameEU extends SearchGame, GameEU { }
export interface SearchGameJP extends SearchGame, GameJP { }

function searchByTitle(db: SearchGame[], title: string, matchQuality: number = matchQualityLimit): SearchGame[] {
    let titleLower = title.toLowerCase();

    excludedChars.forEach(char => titleLower = titleLower.replace(char, ''));
    replaceWithSpaceChars.forEach(char => titleLower = titleLower.replace(char, ' '));
    excludedPhrases.forEach(phrase => titleLower = titleLower.replace(phrase, ''));

    const words = titleLower
        .split(' ')
        .filter(word => excludedWords.indexOf(word) === -1);

    const matches = db.map(game => {
        const gameTitleWords = game.titleLower.trim()
            .replace('  ', ' ')
            .toLowerCase()
            .split(' ')
            .filter(word => excludedWords.indexOf(word) === -1);

        const matchedWords = intersection(words, gameTitleWords);
        return {
            ...game,
            matchQuality:  matchedWords.length / words.length,
        }
    });
        
    const results = orderBy(filter(matches, (game) => game.matchQuality >= matchQuality), ['matchQuality', 'desc']);
    return results;
}

export async function queryByFileUS(filename: string): Promise<SearchSummary> {
    const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');

    let matches = await getQueriedGamesAmerica(`${title}`);
    matches = matches.filter(m => m.platformCode === 'NINTENDO_SWITCH');

    const searchTitle = cleanTitle(title);
    const exactMatches = matches.filter(m => cleanTitle(m.title) === searchTitle);
    const bestGame = exactMatches.length === 1
        ? exactMatches[0]
        : matches.length === 1
            ? matches[0]
            : <QueriedGameUS>{};

    const bestMatch: SearchResult = {
        nsuid: bestGame.nsuid,
        title: bestGame.title,
        description: bestGame.description,
        developer: bestGame.softwareDeveloper || (bestGame.developers || []).join(', ') || bestGame.softwarePublisher || '',
        publisher: bestGame.softwarePublisher || bestGame.softwareDeveloper || (bestGame.developers || []).join(', ') || '',
        players: (bestGame.playerCount || '').replace('+', ''),
        releasedate: bestGame.releaseDateDisplay || '',
        genre: bestGame.genres ? bestGame.genres.join(', ') : '',
    };

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
        // allMatchesJSON: JSON.stringify(matches),
    };
}

export async function queryByFileBR(filename: string): Promise<SearchSummary> {
    const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');

    if (!dbBR) {
        const db = await getGamesBrazil();
        dbBR = db.map((game: GameUS): SearchGameBR => ({
            ...game,
            titleLower: game.title.trim().replace('  ', ' ').toLowerCase(),
            matchQuality: undefined,
        }));
    }

    const searchTitle = cleanTitle(title);
    const matches = <SearchGameBR[]>searchByTitle(dbBR, searchTitle);
    const exactMatches = matches.filter(m => cleanTitle(m.title) === searchTitle);
    const bestGame = exactMatches.length === 1
        ? exactMatches[0]
        : matches.length === 1
            ? matches[0]
            : <SearchGameBR>{};

    const bestMatch: SearchResult = {
        nsuid: bestGame.nsuid,
        title: bestGame.title,
        description: bestGame.description,
        developer: (bestGame.developers || []).join(', ') || (bestGame.publishers || []).join(', '),
        publisher: (bestGame.publishers || []).join(', ') || (bestGame.developers || []).join(', '),
        players: (bestGame.numOfPlayers || '').replace('+', ''),
        releasedate: bestGame.releaseDateDisplay || '',
        genre: bestGame.genres ? bestGame.genres.join(', ') : '',
    };

    return {
        filename,
        title,
        searchTitle,
        matchCount: matches.length,
        exactMatchCount: exactMatches.length,
        bestMatch,
        allMatchesTitles: matches.map(m => m.title).join(', '),
        lang: 'en',
        region: 'br',
        // allMatchesJSON: JSON.stringify(matches),
    };
}

export async function queryByFileEU(filename: string): Promise<SearchSummary> {
    const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');

    if (!dbEU) {
        const db = await getGamesEurope();
        dbEU = db.map((game: GameEU): SearchGameEU => ({
            ...game,
            titleLower: game.title.trim().replace('  ', ' ').toLowerCase(),
            matchQuality: undefined,
        }));
    }

    const searchTitle = cleanTitle(title);
    const matches = <SearchGameEU[]>searchByTitle(dbEU, searchTitle);
    const exactMatches = matches.filter(m => cleanTitle(m.title) === searchTitle);
    const bestGame = exactMatches.length === 1
        ? exactMatches[0]
        : matches.length === 1
            ? matches[0]
            : <SearchGameEU>{};

    const bestMatch: SearchResult = {
        nsuid: (bestGame.nsuid_txt || []).join(''),
        title: bestGame.title,
        description: '',
        developer: bestGame.developer || '',
        publisher: bestGame.publisher || '',
        players: (bestGame.players_to || '').toString(),
        releasedate: bestGame.dates_released_dts && bestGame.dates_released_dts.length > 0 ? bestGame.dates_released_dts[0].toString() : '',
        genre: '',
    };

    return {
        filename,
        title,
        searchTitle,
        matchCount: matches.length,
        exactMatchCount: exactMatches.length,
        bestMatch,
        allMatchesTitles: matches.map(m => m.title).join(', '),
        lang: 'en',
        region: 'eu',
        // allMatchesJSON: JSON.stringify(matches),
    };
}

export async function queryByFileJP(filename: string): Promise<SearchSummary> {
    const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');

    if (!dbJP) {
        const db = await getGamesJapan();
        dbJP = db.map((game: GameJP): SearchGameJP => ({
            ...game,
            titleLower: game.TitleName.toString().trim().replace('  ', ' ').toLowerCase(),
            matchQuality: undefined,
        }));
    }

    const searchTitle = cleanTitle(title);
    const matches = <SearchGameJP[]>searchByTitle(dbJP, searchTitle);
    const exactMatches = matches.filter(m => cleanTitle((m.TitleName || '').toString()) === searchTitle);
    const bestGame = exactMatches.length === 1
        ? exactMatches[0]
        : matches.length === 1
            ? matches[0]
            : <SearchGameJP>{};

    const bestMatch: SearchResult = {
        nsuid: bestGame.InitialCode || '',
        title: (bestGame.TitleName || '').toString(),
        description: bestGame.Memo || '',
        developer: (bestGame.developer || '').toString(),
        publisher: (bestGame.publisher || '').toString(),
        players: '',
        releasedate: bestGame.SalesDate || '',
        genre: '',
    };

    return {
        filename,
        title,
        searchTitle,
        matchCount: matches.length,
        exactMatchCount: exactMatches.length,
        bestMatch,
        allMatchesTitles: matches.map(m => m.title).join(', '),
        lang: 'jp',
        region: 'jp',
        // allMatchesJSON: JSON.stringify(matches),
    };
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
