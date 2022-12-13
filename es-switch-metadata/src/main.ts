import * as fs from 'fs';
import { mergeGamelists, createGamelist, readGamelist, saveGamelist } from './gamelist.js';
import { queryByFileUS } from './search.js';

const dir = 'C:/ROMs/switch/';
const ext = '.xci';

const files = await fs.promises.readdir(dir);
const results = await Promise.all(files
    .filter(filename => filename.endsWith(ext))
    .map(queryByFileUS));

const matched = results.filter(r => r.bestMatch);
const unmatched = results.filter(r => !r.bestMatch);

const oldList = await readGamelist(dir + 'gamelist.xml');
const newList = createGamelist(matched);
const mergedList = mergeGamelists(oldList, newList);
await saveGamelist(dir + 'gamelist.xml', mergedList);

// console.log(unmatched);
