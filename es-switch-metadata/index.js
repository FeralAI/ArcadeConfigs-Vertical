import * as fs from 'fs';
import { getGamesAmerica, getGamesEurope, getGamesJapan, getQueriedGamesAmerica } from 'nintendo-switch-eshop';
import { filter, intersection, orderBy } from 'lodash-es';

const dir = 'C:/ROMs/switch';
const ext = '.xci';
const matchQualityLimit = 0.75;

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

// Define constants
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

// Cache the games database
let db = await getGamesAmerica();
db = db.map(game => ({
    ...game,
    titleLower: game.title.trim().replace('  ', ' ').toLowerCase(),
    titleWords: game.title.trim().replace('  ', ' ').toLowerCase().split(' '),
}));
// console.log(db);
// console.log(filter(db, game => game.title.indexOf('dig') > -1));

// Get list of games
const files = await fs.promises.readdir(dir);
let results = files
    .filter(filename => filename.endsWith(ext))
    .map(filename => {
        const title = filename.substring(0, filename.indexOf('[') - 1).trim().replace('  ', ' ');
        let titleLower = title.toLowerCase();

        excludedChars.forEach(char => titleLower = titleLower.replace(char, ''));
        replaceWithSpaceChars.forEach(char => titleLower = titleLower.replace(char, ' '));
        excludedPhrases.forEach(phrase => titleLower = titleLower.replace(phrase, ''));

        const words = titleLower
            .split(' ')
            .filter(word => excludedWords.indexOf(word) === -1);

        const matches = db.map(game => {
            const matchedWords = intersection(words, game.titleWords);
            return {
                title: game.title,
                titleLower,
                matchedWords, 
                matchQuality: matchedWords.length / words.length,
            };
        });
            
        const results = orderBy(filter(matches, (game) => game.matchQuality >= matchQualityLimit), ['matchQuality', 'desc']);

        return {
            filename,
            title,
            words,
            matches: JSON.stringify(results),
        };
    });
console.log(results);
