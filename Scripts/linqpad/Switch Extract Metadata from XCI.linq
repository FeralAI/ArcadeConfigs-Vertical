<Query Kind="Program">
  <NuGetReference>DokanNet</NuGetReference>
  <NuGetReference Version="0.2.0">LibHac</NuGetReference>
  <Namespace>DokanNet</Namespace>
  <Namespace>DokanNet.Logging</Namespace>
  <Namespace>DokanNet.Native</Namespace>
  <Namespace>LibHac</Namespace>
  <Namespace>LibHac.IO</Namespace>
  <Namespace>LibHac.IO.Save</Namespace>
  <Namespace>LibHac.Npdm</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

// Loosly based on https://github.com/simontime/SwitchExplorer/blob/master/UI.cs

static List<string> RemoveChars = new List<string> { ":" };
static string RomPath = @"C:\retrobat\roms\switch-aa\";
static string ImagePath = @"./media/images/";
static string ProdKeysPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.switch\prod.keys");
static string TitleKeysPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.switch\title.keys");
static string ConsoleKeysPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.switch\console.keys");
static Keyset Keys = ExternalKeys.ReadKeyFile(ProdKeysPath, TitleKeysPath, ConsoleKeysPath);
static string GameListPath = @"C:\retrobat\roms\switch-aa\gamelist.xml";
const string GameListTemplate = @"<?xml version=""1.0""?>
<gameList>
</gameList>";

void Main()
{
	if (!Directory.Exists(ImagePath))
		Directory.CreateDirectory(ImagePath);

	if (File.Exists(GameListPath))
		File.Delete(GameListPath);

	File.WriteAllText(GameListPath, GameListTemplate);

	var files = Directory.GetFiles(RomPath, "*.xci").ToList();
	var metadatas = files.Select(ExtractMetadataXci);
	metadatas.Dump();
	
	foreach (var metadata in metadatas)
	{
		//using (var fs = File.Open(metadata.IconSavePath, FileMode.Create))
		//	metadata.IconImage.Save(fs, ImageFormat.Jpeg);
			
		AddToGamelist(metadata);
	}
}

public class SwitchMetadata
{
	public string Path { get; set; }
	public string Title { get; set; }
	public string Developer { get; set; }
	public string CleanFileTitle
	{
		get
		{
			var fileInfo = new FileInfo(Path);
			var cleanTitle = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("[")).Trim();
			RemoveChars.ForEach(c => cleanTitle = cleanTitle.Replace(c, ""));
			return cleanTitle.Replace("1945Ⅱ", "1945 Ⅱ");
		}
	}
	public string CleanTitle
	{
		get
		{
			var cleanTitle = Title;
			RemoveChars.ForEach(c => cleanTitle = cleanTitle.Replace(c, ""));
			return cleanTitle.Replace("1945Ⅱ", "1945 Ⅱ");
		}
	}
	public string IconPath => System.IO.Path.Combine(ImagePath, $"{CleanFileTitle}.jpg");
	public string IconSavePath => System.IO.Path.Combine(RomPath, $"media\\images\\{CleanFileTitle}.jpg");
	public Image IconImage;
}

public void AddToGamelist(SwitchMetadata data)
{
	var fileInfo = new FileInfo(data.Path);
	var gameListEntry = new
	{
		path = $"./{fileInfo.Name}",
		name = data.Title,
		desc = "",
		developer = data.Developer,
		publisher = "",
		image = data.IconPath,
		releasedate = "",
		//releasedate = DateTime.Now.ToString("yyyyMMddT000000"),
		//genreid = 260, // Shmups
		//genre = "Shoot'em Up",
	};

	// Update game list
	var xmlDoc = new XmlDocument();
	xmlDoc.Load(GameListPath);
	var gameListElement = xmlDoc.DocumentElement;
	var gameXmlElement = CreateGameXmlElement(xmlDoc, gameListEntry.name, gameListEntry.path, gameListEntry.desc, gameListEntry.developer, gameListEntry.publisher, gameListEntry.releasedate, gameListEntry.image);
	gameListElement.AppendChild(gameXmlElement);
	xmlDoc.Save(GameListPath);
}

// You can define other methods, fields, classes and namespaces here
public SwitchMetadata ExtractMetadataXci(string filename)
{
	var inputPFS = File.OpenRead(filename);
	var xci = new Xci(Keys, inputPFS.AsStorage());
	var cnmtNca = new Nca(Keys, xci.SecurePartition.OpenFile(xci.SecurePartition.Files.FirstOrDefault(s => s.Name.Contains(".cnmt.nca"))), false);
	var cnmtPfs = new Pfs(cnmtNca.OpenSection(0, false, IntegrityCheckLevel.None, true));
	var cnmt = new Cnmt(cnmtPfs.OpenFile(cnmtPfs.Files[0]).AsStream());

	var controlEntry = cnmt.ContentEntries.FirstOrDefault(c => c.Type == CnmtContentType.Control);
	var controlNca = new Nca(Keys, xci.SecurePartition.OpenFile($"{controlEntry.NcaId.ToHexString().ToLower()}.nca"), false);
	var rom = new Romfs(controlNca.OpenSection(0, false, IntegrityCheckLevel.None, true));
	var controlStorage = rom.OpenFile(rom.Files.FirstOrDefault(f => f.Name == "control.nacp"));
	var controlNacp = new Nacp(controlStorage.AsStream());

	var description = controlNacp.Descriptions.FirstOrDefault(l => l.Title.Length > 1);
	var iconStorage = rom.OpenFile(rom.Files.FirstOrDefault(f => f.Name.Contains("icon")));

	var metadata = new SwitchMetadata
	{
		Path = filename,
		Title = description.Title.Trim(),
		Developer = description.Developer.Trim(),
		IconImage = Image.FromStream(iconStorage.AsStream()),
	};
	
	return metadata;
}

XmlElement CreateGameXmlElement(XmlDocument xmlDoc, string name, string path, string desc, string developer, string publisher, string releaseDate, string imagePath)
{
	var gameElement = xmlDoc.CreateElement("game");

	var pathElement = xmlDoc.CreateElement("path");
	var nameElement = xmlDoc.CreateElement("name");
	var descElement = xmlDoc.CreateElement("desc");
	var developerElement = xmlDoc.CreateElement("developer");
	var publisherElement = xmlDoc.CreateElement("publisher");
	var dateElement = xmlDoc.CreateElement("releasedate");
	var imageElement = xmlDoc.CreateElement("image");

	pathElement.InnerText = path;
	nameElement.InnerText = name;
	descElement.InnerText = desc;
	developerElement.InnerText = developer;
	publisherElement.InnerText = publisher;
	dateElement.InnerText = releaseDate;
	imageElement.InnerText = imagePath;

	gameElement.AppendChild(pathElement);
	gameElement.AppendChild(nameElement);
	gameElement.AppendChild(descElement);
	gameElement.AppendChild(developerElement);
	gameElement.AppendChild(publisherElement);
	gameElement.AppendChild(dateElement);
	gameElement.AppendChild(imageElement);

	return gameElement;
}