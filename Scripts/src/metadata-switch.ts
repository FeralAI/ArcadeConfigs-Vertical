import * as fs from 'fs';
import { mergeGamelists, readGamelist, saveGamelist } from './modules/gamelist.js';
import { createGamelist, queryByFileBR, queryByFileEU, queryByFileJP, queryByFileUS, SearchSummary } from './modules/search-switch.js';

const dir = 'C:/ROMs/switch/';
const ext = '.xci';

const files = await fs.promises.readdir(dir);
const results = await Promise.all(
    files
        .filter(filename => filename.endsWith(ext))
        .map(queryByFileUS)
);
let unmatched: SearchSummary[] = results.filter(r => !r.bestMatch.title);
let matched: SearchSummary[] = results.filter(r => !!r.bestMatch.title);

console.log(`matched: ${matched.length}, unmatched: ${unmatched.length}`);

const oldList = await readGamelist(dir + 'gamelist.xml');
const newList = createGamelist(matched);
let mergedList = mergeGamelists(oldList, newList);

const updateList = async (q: (value: string, index: number, array: string[]) => Promise<SearchSummary>) => {
    const queryResults = await Promise.all(
        unmatched
            .map(u => u.filename)
            .map(q)
    );

    matched = queryResults.filter(r => !!r.bestMatch.title);
    unmatched = queryResults.filter(r => !r.bestMatch.title);
    console.log(`matched: ${matched.length}, unmatched: ${unmatched.length}`);

    const list = createGamelist(matched);
    const found = list.gameList.game.length > 0;
    if (found)
        mergedList = mergeGamelists(mergedList, list);

    return mergedList;
}

mergedList = await updateList(queryByFileJP);
mergedList = await updateList(queryByFileEU);

await saveGamelist(dir + 'gamelist.xml', mergedList);

console.log(unmatched);
