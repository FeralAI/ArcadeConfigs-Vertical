import axios from 'axios';

const baseUrl = 'https://store.steampowered.com';

export interface SteamMetadata {
    appid: number;
    name: string;
    path: string;
    desc: string;
    developer: string;
    publisher: string;
    releasedate: string;
    image: string;
    marquee: string;
    video: string;
}

interface SteamApiAppInfo {
    [appid: string]: ApiResponse;
}

interface ApiResponse {
    success: boolean;
    data:    Data;
}

interface Data {
    type:                 string;
    name:                 string;
    steam_appid:          number;
    required_age:         number;
    is_free:              boolean;
    controller_support:   string;
    detailed_description: string;
    about_the_game:       string;
    short_description:    string;
    supported_languages:  string;
    header_image:         string;
    website:              string;
    pc_requirements:      PCRequirements;
    mac_requirements:     any[];
    linux_requirements:   any[];
    legal_notice:         string;
    developers:           string[];
    publishers:           string[];
    demos:                Demo[];
    price_overview:       PriceOverview;
    packages:             number[];
    package_groups:       PackageGroup[];
    platforms:            Platforms;
    metacritic:           Metacritic;
    categories:           Category[];
    genres:               Genre[];
    screenshots:          Screenshot[];
    movies:               Movie[];
    recommendations:      Recommendations;
    achievements:         Achievements;
    release_date:         ReleaseDate;
    support_info:         SupportInfo;
    background:           string;
    background_raw:       string;
    content_descriptors:  ContentDescriptors;
}

interface Achievements {
    total:       number;
    highlighted: Highlighted[];
}

interface Highlighted {
    name: string;
    path: string;
}

interface Category {
    id:          number;
    description: string;
}

interface ContentDescriptors {
    ids:   any[];
    notes: null;
}

interface Demo {
    appid:       number;
    description: string;
}

interface Genre {
    id:          string;
    description: string;
}

interface Metacritic {
    score: number;
    url:   string;
}

interface Movie {
    id:        number;
    name:      string;
    thumbnail: string;
    webm:      { [key: string]: string };
    mp4:       { [key: string]: string };
    highlight: boolean;
}

interface PackageGroup {
    name:                      string;
    title:                     string;
    description:               string;
    selection_text:            string;
    save_text:                 string;
    display_type:              number;
    is_recurring_subscription: string;
    subs:                      Sub[];
}

interface Sub {
    packageid:                    number;
    percent_savings_text:         string;
    percent_savings:              number;
    option_text:                  string;
    option_description:           string;
    can_get_free_license:         string;
    is_free_license:              boolean;
    price_in_cents_with_discount: number;
}

interface PCRequirements {
    minimum:     string;
    recommended: string;
}

interface Platforms {
    windows: boolean;
    mac:     boolean;
    linux:   boolean;
}

interface PriceOverview {
    currency:          string;
    initial:           number;
    final:             number;
    discount_percent:  number;
    initial_formatted: string;
    final_formatted:   string;
}

interface Recommendations {
    total: number;
}

interface ReleaseDate {
    coming_soon: boolean;
    date:        string;
}

interface Screenshot {
    id:             number;
    path_thumbnail: string;
    path_full:      string;
}

interface SupportInfo {
    url:   string;
    email: string;
}

export async function getAppDetails(appid: number): Promise<SteamMetadata | null> {
    const appDetailsPath = `/api/appdetails/?appids=${appid}`;
    const response = await axios.get<SteamApiAppInfo>(baseUrl + appDetailsPath);
    if (response.status !== 200)
        return null;

    console.log(response.data);

    return null;
}