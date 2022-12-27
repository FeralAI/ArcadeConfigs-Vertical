<Query Kind="Statements" />

var sourcePath = @"C:\ROMs\mame";
var fullSetPath = @"R:\fbneo";
var dupesFolder = @"C:\ROMs\fbneo-mame";

// Get FBN file list
var sourceFiles = Directory
	.GetFiles(sourcePath, "*.zip", SearchOption.TopDirectoryOnly)
	.Select(f => new FileInfo(f).Name);

// Get list of matching MAME roms
var targetFiles =  Directory
	.GetFiles(fullSetPath, "*.zip", SearchOption.TopDirectoryOnly)
	.Select(f => new FileInfo(f).Name);

var dupeFiles = new List<string>();
var sourceOnlyFiles = new List<string>();

foreach (var sourceFile in sourceFiles)
	if (targetFiles.Contains(sourceFile))
		dupeFiles.Add(sourceFile);
	else
		sourceOnlyFiles.Add(sourceFile);

sourceOnlyFiles.Dump();
dupeFiles.Dump();

foreach (var dupeFile in dupeFiles)
{
	File.Copy(@$"{fullSetPath}\{dupeFile}", @$"{dupesFolder}\{dupeFile}", true);
	File.Delete(@$"C:\ROMs\mame\{dupeFile}");
}


