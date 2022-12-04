<Query Kind="Statements" />

var romDir = @"C:\ROMs\fbneo\";
var newDir = @"C:\ROMs\fbneo-vertical\";
var datFile = @"C:\Utilities\clrmamepro\datfiles\FinalBurn Neo (ClrMame Pro XML, Arcade only).dat";
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

gameList.Dump();

foreach (var file in System.IO.Directory.GetFiles(newDir))
	File.Delete(file);
	
foreach (var game in gameList)
	File.Copy(Path.Combine(romDir, $"{game}.zip"), Path.Combine(newDir, $"{game}.zip"));

