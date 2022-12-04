<Query Kind="Statements" />

var fbneoFiles = Directory.GetFiles(@"C:\ROMs\fbneo").Select(f => new FileInfo(f));
var mameFiles = Directory.GetFiles(@"C:\ROMs\mame").Select(f => new FileInfo(f));

var fbneoNames = fbneoFiles.Select(f => f.Name);
var mameNames = mameFiles.Select(f => f.Name);

var fbneoUnique = fbneoNames.Where(f => !mameNames.Contains(f));
var mameUnique = mameNames.Where(f => !fbneoNames.Contains(f));

fbneoUnique.Dump("FB Neo Unique");
mameUnique.Dump("MAME Unique");

var mameDupes = mameNames.Where(f => fbneoNames.Contains(f));

foreach (var mameDupe in mameDupes)
	File.Move(@$"C:\ROMs\mame\{mameDupe}", @$"C:\ROMs\mame-fbneo-dupes\{mameDupe}");


