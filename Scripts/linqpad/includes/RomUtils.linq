<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\HttpUtils"

const string ArchiveUrlFBNeo = "https://archive.org/download/fbnarcade-fullnonmerged/arcade";
const string ArchiveUrlMAME = "https://archive.org/download/mame-merged/mame-merged";
const string ArchiveUrlMAMESamples = "https://archive.org/download/mame-chds-roms-extras-complete/samples/";
//const string ArchiveUrlMAME = "https://archive.org/download/mame-chds-roms-extras-complete";

public async Task DownloadMissingROMs(IEnumerable<string> missingROMs, string baseUrl, string savePath)
{
	foreach (var missingROM in missingROMs)
		await DownloadBinary($"{baseUrl}/{missingROM}", $"{savePath}\\{missingROM}");
}

public async Task DownloadMissingFBNeo(IEnumerable<string> missingROMs, string savePath)
{
	await DownloadMissingROMs(missingROMs, ArchiveUrlFBNeo, savePath);
}

public async Task DownloadMissingMAME(IEnumerable<string> missingROMs, string savePath)
{
	await DownloadMissingROMs(missingROMs, ArchiveUrlMAME, savePath);
}

public async Task DownloadMissingMAMESamples(IEnumerable<string> missingROMs, string savePath)
{
	await DownloadMissingROMs(missingROMs, ArchiveUrlMAMESamples, savePath);
}