import * as fs from 'fs';
import { queryByFileBR, queryByFileEU, queryByFileJP, queryByFileUS } from './search.js';

const ext = '.xci';

// Get list of games
// const files = await fs.promises.readdir(dir);
const files = [
    'Arcade Archives FLAK ATTACK [0100354011914000] [v0].xci',
    'Psikyo Collection Vol.1 [01009F100BC52000] [v0].xci',
    'Psikyo Collection Vol.2 [01009D400C4A8000] [v65536] (1G+1U).xci',
    'Psikyo Collection Vol.3 [0100C8D00D142000] [v0].xci',
    'STRIKERS1945Ⅱ for Nintendo Switch [0100720008ED2000] [v196608] (1G+1U).xci',
    'エスプレイドΨ [0100F9600E746000] [v327680] (1G+1U).xci',
];

const results = await Promise.all(
    files
        .filter(filename => filename.endsWith(ext))
        .map(async f => await queryByFileJP(f))
);

console.log(results);
