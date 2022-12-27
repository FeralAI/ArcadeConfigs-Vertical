import * as fs from 'fs';

const romDir = 'C:/ROMs/';
const saveDir = '../../ROMs/';

// Do this:
// 1. Find folders that contain gamelist.xml
// 2. For each folder:
//     a.  Create folder in saveDir if not exist
//     b.  Copy gamelist.xml and media folder to folder in saveDir

let folders: fs.Dirent[] | null | undefined = await fs.promises.readdir(romDir, { withFileTypes: true });
folders = folders.filter(f => f.isDirectory());

let fileLists = await Promise.all(folders.map(async f => ({ folder: f.name, files: await fs.promises.readdir(romDir + f.name) })));
folders = null;
fileLists = fileLists.filter(l => l.files.indexOf('gamelist.xml') > -1);
console.log(fileLists.map(l => l.folder));

const results = await Promise.all(fileLists.map(async l => {
    const sourceFolder = romDir + l.folder + '/';
    const targetFolder = saveDir + l.folder + '/';
    let folderExists = false;
    try {
        await fs.promises.access(targetFolder);
        folderExists = true;
    }
    catch (error: any) {
        // Ignore "no such file or directory error"
        if (error.errno !== -4058) {
            console.log(error);
            return { list: l, success: false };
        }
    }

    if (folderExists) {
        try {
            console.log('deleting folder ' + targetFolder);
            await fs.promises.rm(targetFolder, { recursive: true, force: true });
        }
        catch (error) {
            console.log(error);
            return { list: l, success: false };
        }
    }

    try {
        await fs.promises.mkdir(targetFolder);
        await fs.promises.cp(sourceFolder + 'gamelist.xml', targetFolder + 'gamelist.xml', { force: true });
        await fs.promises.cp(sourceFolder + 'media/', targetFolder + 'media/', { recursive: true, force: true });
    }
    catch (error) {
        console.log(error);
        return { list: l, success: false };
    }

    return { list: l, success: true };
}));

console.log(results);
