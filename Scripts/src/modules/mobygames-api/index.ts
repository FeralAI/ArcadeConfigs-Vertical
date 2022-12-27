import axios from 'axios';

namespace MobyGamesApi {
    const baseUrl = 'https://api.mobygames.com/v1';
    const apiKey = 'DsLqesiA7RGV3Z/y4uf3Aw==';

    /****************************** Data models ******************************/
    export interface MobyGamesGame {
        alternate_titles?:   MobyGamesAltTitle[];
        description?:        string;
        game_id:             number;
        genres?:             MobyGamesGenre[];
        moby_score?:         number;
        moby_url:            string;
        num_vote?:           number;
        official_url?:       null | string;
        platforms?:          MobyGamesPlatform[];
        sample_cover?:       MobyGamesSample | null;
        sample_screenshots?: MobyGamesSample[];
        title:               string;
    }

    export enum MobyGamesFormat {
        id     = 'id',
        brief  = 'brief',
        normal = 'normal',
        full   = 'full',
    }

    export interface MobyGamesAltTitle {
        description: string;
        title:       string;
    }

    export interface MobyGamesGenre {
        genre_category:    string;
        genre_category_id: number;
        genre_id:          number;
        genre_name:        string;
        genre_description?: string;
    }

    export interface MobyGamesGroup {
        group_description: string;
        group_id:          number;
        group_name:        string;
    }

    export interface MobyGamesPlatform {
        platform_id:   number;
        platform_name: string;
        first_release_date?: string;
    }

    export interface MobyGamesSample {
        height:          number;
        image:           string;
        platforms?:      string[];
        thumbnail_image: string;
        width:           number;
        caption?:        string;
    }

    export interface MobyGamesGamePlatformDetails {
        attributes:         any[];
        first_release_date: string;
        game_id:            number;
        patches:            any[];
        platform_id:        number;
        platform_name:      string;
        ratings:            any[];
        releases:           MobyGamesRelease[];
    }

    export interface MobyGamesRelease {
        companies:     MobyGamesCompany[];
        countries:     string[];
        description:   null;
        product_codes: any[];
        release_date:  string;
    }

    export interface MobyGamesCompany {
        company_id:   number;
        company_name: string;
        role:         string;
    }

    export interface MobyGamesGamePlatformScreenshots {
        screenshots: MobyGamesScreenshot[];
    }

    export interface MobyGamesScreenshot {
        caption:         string;
        height:          number;
        image:           string;
        thumbnail_image: string;
        width:           number;
    }

    export interface MobyGamesCoverGroup {
        comments:       string;
        countries:      string[];
        cover_group_id: number;
        covers:         MobyGamesCover[];
        packaging:      MobyGamesPackaging;
    }

    export interface MobyGamesCover {
        comments:        null;
        description:     null;
        height:          number;
        image:           string;
        scan_of:         string;
        thumbnail_image: string;
        width:           number;
    }

    export interface MobyGamesPackaging {
        packaging_id:   number;
        packaging_name: string;
    }

    /****************************** API response models ******************************/
    export interface MobyGamesGamesIdListResponse {
        games: number[];
    }

    export interface MobyGamesGamePlatformCoversResponse {
        cover_groups: MobyGamesCoverGroup[];
    }

    export interface MobyGamesGamePlatformDetailsResponse extends MobyGamesGamePlatformDetails {

    }

    export interface MobyGamesGamePlatformScreenshotsResponse extends MobyGamesGamePlatformScreenshots {

    }

    export interface MobyGamesGameResponse extends MobyGamesGame {

    }

    export interface MobyGamesGamesResponse {
        games: MobyGamesGame[];
    }

    export interface MobyGamesGenreResponse {
        genres: MobyGamesGenre[];
    }

    export interface MobyGamesGroupResponse {
        groups: MobyGamesGroup[];
    }

    export interface MobyGamesPlatformResponse {
        platforms: MobyGamesPlatform[];
    }

    /****************************** API request models ******************************/
    export interface MobyGamesRandomParams {
        format?: MobyGamesFormat;
        limit?: number;
    }

    export interface MobyGamesRecentParams {
        format?: MobyGamesFormat;
        limit?: number;
        offset?: number;
        age?: number;
    }

    export interface MobyGamesGamesParams {
        format?: MobyGamesFormat;
        limit?: number;
        offset?: number;
        platform?: number[] | number;
        genre?: number[] | number;
        group?: number[] | number;
        title?: string;
    }

    function validateAge(age: number): number {
        return (age < 1 || age > 21) ? 21 : age;
    }

    function validateLimit(limit: number): number {
        return (limit < 1 || limit > 100) ? 100 : limit;
    }

    function validateOffset(offset: number): number {
        return (offset < 1 || offset > 100) ? 100 : offset;
    }

    /****************************** API functions ******************************/
    async function get<T>(url: string, params: { [key:string]: any } = {}) {
        Object.keys(params).forEach(p => {
            switch (p) {
                case 'age'   : params[p] = validateAge(params[p]); break;
                case 'limit' : params[p] = validateLimit(params[p]); break;
                case 'offset': params[p] = validateOffset(params[p]); break;
            }
        });

        const response = await axios.get<T>(url, { params: { ...params, api_key: apiKey }});
        if (response.status === 200)
            return <T>response.data;
        
        return null;
    }

    export async function getGenres(): Promise<MobyGamesGenreResponse | null> {
        return await get<MobyGamesGenreResponse>(`${baseUrl}/genres`);
    }

    export async function getGroups(limit: number = 100, offset: number = 0): Promise<MobyGamesGroupResponse | null> {
        return await get<MobyGamesGroupResponse>(`${baseUrl}/groups`, { limit, offset });
    }

    export async function getPlatforms(): Promise<MobyGamesPlatformResponse | null> {
        return await get<MobyGamesPlatformResponse>(`${baseUrl}/platforms`);
    }

    export async function getGames(params: MobyGamesGamesParams = {}): Promise<MobyGamesGamesResponse | MobyGamesGamesIdListResponse | null> {
        return await get<MobyGamesGamesResponse | MobyGamesGamesIdListResponse>(`${baseUrl}/games`, params);
    }

    export async function getGame(id: number, format: MobyGamesFormat = MobyGamesFormat.full): Promise<MobyGamesGameResponse | null> {
        return await get<MobyGamesGameResponse>(`${baseUrl}/games/${id}`, { format });
    }

    export async function getGamePlatforms(id: number): Promise<MobyGamesPlatformResponse | null> {
        return await get<MobyGamesPlatformResponse>(`${baseUrl}/games/${id}/platforms`);
    }

    export async function getGamePlatformCovers(id: number, platformId: number): Promise<MobyGamesGamePlatformCoversResponse | null> {
        return await get<MobyGamesGamePlatformCoversResponse>(`${baseUrl}/games/${id}/platforms/${platformId}/covers`);
    }

    export async function getGamePlatformDetails(id: number, platformId: number): Promise<MobyGamesGamePlatformDetailsResponse | null> {
        return await get<MobyGamesGamePlatformDetailsResponse>(`${baseUrl}/games/${id}/platforms/${platformId}`);
    }

    export async function getGamePlatformScreenshots(id: number, platformId: number): Promise<MobyGamesGamePlatformScreenshotsResponse | null> {
        return await get<MobyGamesGamePlatformScreenshotsResponse>(`${baseUrl}/games/${id}/platforms/${platformId}/screenshots`);
    }

    export async function getGamesRandom(params: MobyGamesRandomParams = {}): Promise<MobyGamesGamesResponse | MobyGamesGamesIdListResponse | null> {
        return await get<MobyGamesGamesResponse | MobyGamesGamesIdListResponse>(`${baseUrl}/games/random`, params);
    }

    export async function getGamesRecent(params: MobyGamesRecentParams = {}): Promise<MobyGamesGamesResponse | MobyGamesGamesIdListResponse | null> {
        return await get<MobyGamesGamesResponse | MobyGamesGamesIdListResponse>(`${baseUrl}/games/recent`, params);
    }
}

export default MobyGamesApi;
