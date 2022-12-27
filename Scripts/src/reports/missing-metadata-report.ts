import * as fs from 'fs';
import glob from 'glob-promise';
import { readGamelist } from '../modules/gamelist.js';
import { parseAsync } from 'json2csv';

interface MetadataValidationGameResult {
    system: string;
    name: string;
    hasDesc: boolean;
    hasDeveloper: boolean;
    hasReleaseDate: boolean;
    hasRating: boolean;
    hasImage: boolean;
    hasVideo: boolean;
}

interface MetadataValidationSystemResult {
    system: string;
    filename: string;
    missing: MetadataValidationGameResult[];
}

const path = 'C:/arcade/roms';
const gamelistFilename = 'gamelist.xml';
const files = await glob.promise(`${path}/**/${gamelistFilename}`);

const systemMap: MetadataValidationSystemResult[] = await Promise.all(files.map(async filename => {
    const gamelist = await readGamelist(filename);
    const system = filename.replace(`${path}/`, '').replace(`/${gamelistFilename}`, '');
    const missing: MetadataValidationGameResult[] = gamelist.gameList.game
        .map(g => {
            return {
                system,
                name: g.name[0],
                hasDesc: !!g.desc,
                hasDeveloper: !!g.developer,
                hasReleaseDate: !!g.releasedate,
                hasRating: !!g.rating && g.rating != '0',
                hasImage: !!g.image,
                hasVideo: !!g.video,
            };
        })
        .filter(r => !r.hasDesc || !r.hasDeveloper || !r.hasReleaseDate || !r.hasRating || !r.hasImage || !r.hasVideo);

    return {
        system,
        filename,
        missing,
    };
}));

const results = systemMap
    .filter(r => r.missing.length > 0)
    .reduce((rr: MetadataValidationGameResult[], r) => {
        rr.push(...r.missing);
        return rr;
    }, []);

const fields = ['system', 'name', 'hasDesc', 'hasDeveloper', 'hasReleaseDate', 'hasRating', 'hasImage', 'hasVideo'];
const opts = { fields };
const transformOpts = { highWaterMark: 8192 };
const csv = await parseAsync(results, opts, transformOpts);
const csvFilename = 'C:/arcade/logs/missing-metadata.csv';
await fs.promises.writeFile(csvFilename, csv);
console.log(`Report written to ${csvFilename}`);
