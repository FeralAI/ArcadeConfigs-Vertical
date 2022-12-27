import { initGamelist, mergeGamelists, readGamelist, saveGamelist } from "./modules/gamelist.js";
import { downloadMetadatasSwitch, getGameNamesSwitchAA, matchGames } from "./modules/hamster.js";

const createNewList = false;
const removePrefix = true;
const downloadPath = 'C:\\retrobat\\roms\\switch-aa-new\\media\\images';
const romPath = 'C:\\retrobat\\roms\\switch-aa\\';
const gamelistPath = `${romPath}\\gamelist.xml`;

// Test code
// const game = {
//     name: 'DRAGON SPIRIT',
//     fullName: 'Arcade Archives DRAGON SPIRIT [010092201711E000] [v0]',
//     saveName: 'Arcade Archives DRAGON SPIRIT',
//     fullPath: 'C:\\retrobat\\roms\\switch-aa\\Arcade Archives DRAGON SPIRIT [010092201711E000] [v0].xci',
//     nameSlug: 'dragon-spirit',
// };
// downloadMetadataSwitchAA(game);

let games = await getGameNamesSwitchAA(romPath);        // Get the ROM list to generate metadata for
games = await matchGames(games);                        // Run matching against game list page
if (createNewList) await initGamelist(gamelistPath);    // Force create new gamelist if desired
let gamelist = await readGamelist(gamelistPath);        // Get existing gamelist for updating
const newlist = await downloadMetadatasSwitch(games, removePrefix); // Download metadata
gamelist = mergeGamelists(gamelist, newlist);           // Merge gamelists
await saveGamelist(gamelistPath, gamelist);             // Save it

// downloadIconsSwitchAA(games);             // Download icons

// TODO: Download and crop screenshots
