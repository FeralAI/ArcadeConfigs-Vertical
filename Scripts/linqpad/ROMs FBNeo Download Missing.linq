<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "includes\RomUtils"

var romPath = @"X:\New\fbneo";
var missingROMs = new List<string>
{
	"fantasy",
	"ibarao",
	"natodefa",
	"pwrinst2a",
	"tr606drumkit",
}
.Select(r => $"{r}.zip");

await DownloadMissingFBNeo(missingROMs, romPath);
