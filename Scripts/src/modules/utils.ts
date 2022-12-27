import axios from 'axios';
import * as fs from 'fs';

const excludedPhrases = ['arcade archives ', ' for nintendo switch'];
const replaceWithSpace = [' - '];

export function cleanHtml(html: string): string {
    return html?.replace(/\s/g,' ')
                .replace(/\&nbsp\;/g, ' ')
                .replace(/\&amp\;/g, '\&')
                .replace(/\&gt\;/g, '\>')
                .replace(/\&lt\;/g, '\<')
                .replace(/\&quot\;/g, '\'')
                .replace(/\&\#39\;/g, '\'')
                .trim();
}

export function cleanTitle(title: string): string {
    title = title.toLowerCase();
    replaceWithSpace.forEach(x => title = title.replace(x, ' '));
    excludedPhrases.forEach(x => title = title.replace(x, ''));
    title = title.replace('1945ⅱ', '1945 ⅱ'); // Special handling for Strikers 1945 II
    title = title.replace('ⅲ', 'iii'); // Special handling for Strikers 1945 III
    title = title.replace(/[:-\\|\/\.]/g, '').trim(); // Remove non-word chars

    return title;
}

export async function createDirectory(path: string, forceOverwrite: boolean = false): Promise<boolean> {
    let folderExists = false;

    try {
        await fs.promises.access(path);
        folderExists = true;
    }
    catch (error: any) {
        // Ignore "no such file or directory error"
        if (error.errno !== -4058) {
            console.log(error);
            return false;
        }
    }

    if (folderExists && forceOverwrite) {
        await fs.promises.rm(path, { force: true, recursive: true });
        folderExists = false;
    }

    if (!folderExists)
        folderExists = !!(await fs.promises.mkdir(path, { recursive: true }));

    return folderExists;
}

export async function deleteFile(path: string): Promise<void> {
    await fs.promises.rm(path, { force: true });
}

export async function downloadFile(url: string, path: string): Promise<boolean> {
    try {
        const response = await axios.get(url, { responseType: 'arraybuffer' });
        if (response.status > 299) {
            console.log('Request failed for ' + url);
            return false;
        }

        await fs.promises.writeFile(path, response.data);
        return true;
    }
    catch (error) {
        console.log(error);
        return false;
    }
}

export async function saveToFile(path: string, content: string): Promise<void> {
    await fs.promises.writeFile(path, content);
}
