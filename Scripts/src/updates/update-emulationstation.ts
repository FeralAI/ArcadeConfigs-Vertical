import AdmZip from 'adm-zip';
import axios from 'axios';
import * as fs from 'fs';

const esPath = '..\\frontends\\EmulationStation\\';

// Backup current ES
const backupPath = '..\\backup\\EmulationStation.zip';
let backupExists = false;
try {
    await fs.promises.access(backupPath);
    backupExists = true;
}
catch (error: any) {
    // Ignore "no such file or directory error", fail on everything else
    if (error.errno !== -4058) {
        console.log(error);
        process.exit(error.errno);
    }
}
if (backupExists)
    await fs.promises.rm(backupPath, { force: true });

const backupZip = new AdmZip();
await backupZip.addLocalFolderPromise(esPath, { });
await backupZip.writeZipPromise(backupPath);

// Download new ES
const url = 'https://github.com/fabricecaruso/batocera-emulationstation/releases/download/continuous-master/EmulationStation-Win32.zip';
const response = await axios.get(url, { responseType: 'arraybuffer' });
const zip = new AdmZip(response.data);
zip.extractAllTo(esPath, true, true);
