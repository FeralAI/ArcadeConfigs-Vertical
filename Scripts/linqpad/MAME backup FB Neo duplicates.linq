<Query Kind="Statements" />

var fbneoPath = @"C:\ROMs\fbneo";
var mameFullSetPath = @"R:\mame";
var mameDupesFolder = @"C:\ROMs\mame-fbneo";

// Get FBN file list
var fbneoFiles = Directory
	.GetFiles(fbneoPath, "*.zip", SearchOption.TopDirectoryOnly)
	.Select(f => new FileInfo(f).Name);

// Get list of matching MAME roms
var mameFiles =  Directory
	.GetFiles(mameFullSetPath, "*.zip", SearchOption.TopDirectoryOnly)
	.Select(f => new FileInfo(f).Name);

var dupeFiles = new List<string>();
var fbneoOnlyFiles = new List<string>();

foreach (var fbneoFile in fbneoFiles)
	if (mameFiles.Contains(fbneoFile))
		dupeFiles.Add(fbneoFile);
	else
		fbneoOnlyFiles.Add(fbneoFile);

fbneoOnlyFiles.Dump();
dupeFiles.Dump();

foreach (var dupeFile in dupeFiles)
	File.Copy(@$"{mameFullSetPath}\{dupeFile}", @$"{mameDupesFolder}\{dupeFile}",  true);
