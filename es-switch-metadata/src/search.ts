import { GameUS, getQueriedGamesAmerica, QueriedGameUS } from "nintendo-switch-eshop";
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

export interface SearchResult {
    filename: string;
    title: string;
    searchTitle: string;
    matchCount: number;
    exactMatchCount: number;
    bestMatch: QueriedGameUS,
    allMatchesTitles: string,
    lang: string,
    region: string,
    [key: string]: any;
}

export interface SearchGameUS extends GameUS {
    titleLower: string;
    titleWords: string[];
}

export async function queryByFileUS(filename: string): Promise<SearchResult> {
    const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');

    let matches = await getQueriedGamesAmerica(`${title}`);
    matches = matches.filter(m => m.platformCode === 'NINTENDO_SWITCH');
    const searchTitle = cleanTitle(title);
    const exactMatches = matches.filter(m => cleanTitle(m.title) === searchTitle);

    const bestMatch = exactMatches.length === 1
        ? exactMatches[0]
        : matches.length === 1
            ? matches[0]
            : <QueriedGameUS>{};

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