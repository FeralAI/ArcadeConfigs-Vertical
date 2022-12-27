<Query Kind="Statements" />

var romDir = @"X:\ROMs\mame\";
var newDir = @"X:\ROMs\mame-vertical\";
var datFile = @"C:\retrobat\dats\mame250.xml";
var settings = new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Parse  };
var reader = XmlReader.Create(datFile, settings);
var gameList = new List<string>();
string gameName = null;
while (await reader.ReadAsync())
{	
	switch (reader.NodeType)
	{
		case XmlNodeType.Element:
			if (reader.Name == "machine")
				gameName = reader.GetAttribute("name");
			if (reader.Name == "display" && (reader.GetAttribute("rotate") == "90" || reader.GetAttribute("rotate") == "270"))
				gameList.Add(gameName);
			break;
		case XmlNodeType.EndElement:
			if (reader.Name == "game")
				gameName = null;
			break;
		default:
			break;
	}
}

gameList.Count.Dump();
	
foreach (var game in gameList)
{
	var currentPath = Path.Combine(romDir, $"{game}.zip");
	var newPath = Path.Combine(newDir, $"{game}.zip");
	
	if (File.Exists(currentPath))
	{
		if (File.Exists(newPath))
			File.Delete(newPath);
			
		File.Copy(currentPath, newPath);
	}
}
