/********* WIP *********/

import * as fs from 'fs';
import { getAppDetails, SteamMetadata } from './modules/steam-api.js';

const ignoredAppIds = [228980];
const ignoredAppNames = ['Steamworks Common Redistributables'];

const path = 'C:\\Program Files (x86)\\Steam\\steamapps\\';
let files = await fs.promises.readdir(path);
files = files.filter(f => f.startsWith('appmanifest_'));

const games = await Promise.all(files.map(async (f): Promise<SteamMetadata | null> => {
    const content = await fs.promises.readFile(path + f);
    const stringContent = content.toString();
    const lines = stringContent
        .split('\n')
        .map(l => l.trim())
        .filter(l => l.startsWith('"appid"') || l.startsWith('"name"'));

    const appidString = lines[0].split('\t\t')[1].replace('"', '');
    const appid = parseInt(appidString);
    const name = lines[1].split('\t\t')[1].replace('"', '');
    const details = await getAppDetails(appid);
    
    
    // return { appid, name };
    return null;
}));

// console.log(games);