const excludedPhrases = ['arcade archives ', ' for nintendo switch'];
const replaceWithSpace = [' - '];

export function cleanTitle(title: string): string {
    title = title.toLowerCase();
    replaceWithSpace.forEach(x => title = title.replace(x, ' '));
    excludedPhrases.forEach(x => title = title.replace(x, ''));
    title = title.replace('ⅱ', ' ii'); // Special handling for Strikers 1945 II
    title = title.replace('ⅲ', 'iii'); // Special handling for Strikers 1945 III
    title = title.replace(/[:-\\|\/\.]/g, '').trim(); // Remove non-word chars

    return title;
}
