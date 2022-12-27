import * as fs from 'fs';
import axios from "axios";
import cheerio from 'cheerio';
import path from "path";
import { Gamelist, GamelistGame, saveGamelist } from "./gamelist.js";
import { cleanHtml, createDirectory, downloadFile } from "./utils.js";

const prefixAA = 'Arcade Archives ';
const prefixACA = 'ACA NEOGEO ';

const baseUrl = 'http://www.hamster.co.jp/american_hamster/arcadearchives/switch';
const titleListUrlAA = `${baseUrl}/title_list.htm`;
const titleListUrlACA = `${baseUrl}/title_list_neogeo.htm`;
const imageBaseUrl = `${baseUrl}/images/title`;

// Combine imageBaseUrl with the following suffixes for various images:
// * /<nameSlug>/<nameSlug>_01.jpg - title image, e.g. /dragon-spirit/dragon-spirit_01.jpg
// * /<nameSlug>/ss_##.jpg - full size image, e.g. /dragon-spirit/ss_01.jpg
// * /<nameSlug>/ss_##s.jpg - thumbnails 01-04, e.g. /dragon-spirit/ss_01s.jpg

export enum TitleType {
    AA,  // Arcade Archives
    ACA, // Arcade Archives Neo Geo
}

interface CleanTagElement extends cheerio.TagElement {
    cleanAlt: string;
};

export interface GameName {
    name: string;
    fullName: string;
    saveName: string;
    fullPath: string;
    relativePath: string;
    nameSlug?: string;
    titleType: TitleType;
}

function getNameSlug(thumbnailUrl: string) {
    return thumbnailUrl
        ?.split('/')
        .at(-1)
        ?.replace('icon_', '')
        .replace('.jpg', '');
}

export async function downloadMetadataSwitch(gameName: GameName, removeAAPrefix: boolean = false): Promise<GamelistGame | null> {
    // Cache the game info page
    const infoPageUrl = `${baseUrl}/${gameName.nameSlug}.htm`;
    let html = '';

    try {
        const response = await axios.get(infoPageUrl);
        html = response.data;
    }
    catch (error) {
        console.log('Page not found: ' + infoPageUrl);
        return null;
    }
    
    const $ = cheerio.load(html);
    const genre: string[] = [];
    const players: string[] = [];
    const developer: string[] = [];
    const releasedate: string[] = [];
    const releasedateAA: string[] = [];

    // Find td with innerText from this list, then get next sibling td innerText for value
    $('td').each((_index, elem) => {
        const label = cleanHtml($(elem).text());
        switch (label) {
            case 'Release Date': releasedateAA.push($(elem).next().text()); break;
            case 'Genre': genre.push($(elem).next().text()); break;
            case 'Player': players.push($(elem).next().text()); break;
            case 'Brand Name': developer.push($(elem).next().text()); break;
            case 'Age': releasedate.push($(elem).next().text()); break;
        }
    });

    // Description:
    // Look for 'div[id="titleOverview"]', get first child (should be a span),
    // Ignore single <br>, double <br> is a new paragraph
    let lines = $('div[id="titleOverview"]').children('span').first().text()
        ?.split('\n')
        .map(s => s?.trim());

    lines = lines.map(l => l.replaceAll('  ', ' '));
    let joined = lines.join(' ').trim().replaceAll('  ', '\n\n');
    const desc = [joined];

    // Screenshot urls:
    // href of A tag is full size
    // src of the IMG tag is the thumbnail
    const coverUrl = `${imageBaseUrl}/${gameName.nameSlug}/${gameName.nameSlug}_01.jpg`;
    const screenshotUrls = $('div[id="screenshot"]').children('div').children('a').map((index, elem) => {
         return `${baseUrl}/${$(elem).attr('href')}`;
    }).toArray();

    const game: GamelistGame = {
        $: { id: gameName.nameSlug || '', source: 'Hamster AA Switch Webpage' },
        name: [removeAAPrefix ? gameName.name : gameName.saveName],
        path: [gameName.relativePath],
        desc,
        image: [`./media/images/${gameName.saveName}.jpg`],
        publisher: ['Hamster'],
        releasedate,
        developer,
        players,
        genre,
        lang: ['en'],
        region: ['us'],
        coverUrl,
        screenshotUrls,
    };

    // console.log(game);
    return game;
}

export async function downloadMetadatasSwitch(games: GameName[], removeAAPrefix: boolean = false): Promise<Gamelist> {
    const gamelistGames = await Promise.all(games.map(g => downloadMetadataSwitch(g, removeAAPrefix)));
    const game: GamelistGame[] = gamelistGames.filter(g => !!g).map(g => <GamelistGame>g);
    const gamelist: Gamelist = {
        gameList: {
            provider: {
                System: 'Arcade Archives for Nintendo Switch',
                software: 'custom',
                database: 'Hamster Website',
                web: 'http://www.hamster.co.jp/american_hamster/arcadearchives/switch/',
            },
            game,
        }
    };

    // Remove unused props
    game.forEach(g => {
        delete g.screenshotUrls;
        delete g.coverUrl;
    });

    return gamelist;
}

export async function downloadIconSwitchAA(game: GameName, downloadPath: string): Promise<boolean> {
    const fileName = path.join(downloadPath, `${game.saveName}.jpg`);
    const imgUrl = `${imageBaseUrl}/${game.nameSlug}/${game.nameSlug}_01.jpg`;
    return await downloadFile(imgUrl, fileName);
}

export async function downloadIconsSwitchAA(games: GameName[], downloadPath: string): Promise<void> {
    // Ensure download path exists
    const downloadPathExists = await createDirectory(downloadPath);
    if (!downloadPathExists) {
        console.log('Error creating download path.');
        process.exit(-1);
    }

    games.forEach(g => downloadIconSwitchAA(g, downloadPath));
}

export async function getGameNamesSwitchAA(romPath: string): Promise<GameName[]> {
    let files = await fs.promises.readdir(romPath);
    files = files.filter(f => (f.startsWith(prefixAA) || f.startsWith(prefixACA)) && f.endsWith('.xci'));
    
    let games: GameName[] = await Promise.all(files.map(async (fullPath): Promise<GameName> => {
        const pathInfo = path.parse(fullPath);
        const fullName = pathInfo.name;
        const relativePath = './' + pathInfo.name + pathInfo.ext;
        const saveName = fullName.substring(0, fullName.indexOf(' [')).trim();
        const name = saveName
            .replace(prefixAA, '')
            .replace(prefixACA, '')
            .trim();
        const titleType = saveName.startsWith(prefixACA) ? TitleType.ACA : TitleType.AA;

        return { fullName, saveName, fullPath, relativePath, name, titleType };
    }));
    // console.log(games);

    return games;
}

async function getTitleList(url: string): Promise<CleanTagElement[]> {
    // Get html elements with game info for matching
    const response = await axios.get(url);
    const html = response.data;
    const $ = cheerio.load(html);
    const data: CleanTagElement[] = $('.titleName img').toArray().map(elem => {
        const tagElem = <cheerio.TagElement>elem;
        return {
            ...tagElem,
            cleanAlt: cleanHtml(tagElem.attribs['alt']),
        };
    });

    return data;
}

export async function matchGames(games: GameName[]): Promise<GameName[]> {
    // Get html elements with game info for matching
    const dataAA = await getTitleList(titleListUrlAA);
    const dataACA = await getTitleList(titleListUrlACA);

    const matched: GameName[] = [];
    const unmatched: string[] = [];

    games.forEach(async g => {
        let matches = [
            ...dataAA.filter(elem => elem.cleanAlt === g.name),
            ...dataACA.filter(elem => elem.cleanAlt === g.name),
        ];

        if (matches.length == 0) {
            unmatched.push(g.name);
            return;
        }

        const match = matches[0];
        g.nameSlug = getNameSlug(match.attribs['src']);
        matched.push(g);
    });

    console.log('Matched games: ' + matched.length);
    console.log(`Not found: ${unmatched.join(', ')}`);
    return matched;
}
