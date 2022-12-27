<Query Kind="Statements" />

var files = Directory.GetFiles(@"C:\ROMs\xbox360\", "*.360");
var titleIds = files
	.Select(f => new { Path = f, Name = new FileInfo(f).Name })
	.Select(n => new { Path = n.Path, TitleId = n.Name.Substring(n.Name.IndexOf("[") + 1, 8) });
	
titleIds.Dump();