<Query Kind="Statements" />

var romDir = @"X:\ROMs\fbneo\";
var newDir = @"X:\ROMs\fbneo-vertical\";
var datFile = @"C:\retrobat\dats\FinalBurn Neo v1.0.0.3 2022-12-20.dat";
var settings = new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Parse  };
var reader = XmlReader.Create(datFile, settings);
var gameList = new List<string>();
string gameName = null;
while (await reader.ReadAsync())
{	
	switch (reader.NodeType)
	{
		case XmlNodeType.Element:
			if (reader.Name == "game")
				gameName = reader.GetAttribute("name");
			if (reader.Name == "video" && reader.GetAttribute("orientation") == "vertical")
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
