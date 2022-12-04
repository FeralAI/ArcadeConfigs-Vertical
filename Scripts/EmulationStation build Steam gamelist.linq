<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var appIds = new List<int>
	{
		1718160, // Crimzon_Clover_World_EXplosion
		280560,  // Danmaku_Unlimited_2
		450950,  // Danmaku_Unlimited_3
		422510,  // DEMONS_TILT
		464450,  // DoDonPachi_Resurrection
	};
	
	// https://store.steampowered.com/app/278510/JUDGEMENT_SILVERSWORD__Resurrection/
	var appId = 278510;
	var imagePath = @"R:\steam\media";
	var appDetailsPath = "/api/appdetails/?appids=";
	
	var client = new HttpClient();
	client.BaseAddress = new Uri("https://store.steampowered.com");
	var response = await client.GetAsync(appDetailsPath + appId);
	var result = await response.Content.ReadAsStringAsync();
	var jsonDoc = JsonDocument.Parse(result);
	var jsonRoot = jsonDoc.RootElement;
	var gameElement = jsonRoot.GetProperty(appId.ToString());
	var gameDataElement = gameElement.GetProperty("data");
	
	var gameAppId = gameDataElement.GetProperty("steam_appid").GetInt32();
	var gameListEntry = new
	{
		path = $"./{gameAppId}.sid",
		name = gameDataElement.GetProperty("name").GetString(),
		desc = gameDataElement.GetProperty("short_description").GetString(),
		releasedate = DateTime.Parse(gameDataElement.GetProperty("release_date").GetProperty("date").GetString()).ToString("yyyyMMddT000000"),
		image = gameDataElement.GetProperty("screenshots").EnumerateArray().First(_ => _.GetProperty("path_full").GetString().Contains(".jpg")).GetProperty("path_full").GetString(),
		marquee = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameAppId}/header.jpg",
		fileName = ReplaceInvalidFileChars(gameDataElement.GetProperty("name").GetString()),
	};
	
	gameListEntry.Dump();

	// Download the images
	await DownloadImage(gameListEntry.image, @$"{imagePath}\screenshots\{gameListEntry.fileName}.jpg");
	await DownloadImage(gameListEntry.marquee, @$"{imagePath}\marquees\{gameListEntry.fileName}.jpg");
	
	// Create SID file
	var sidFile = @$"R:\steam\{gameAppId}.sid";
	if (!File.Exists(sidFile))
		File.Create(sidFile);
	
	// Update game list
	var gameListPath = @"R:\steam\gamelist.xml";
	var xmlDoc = new XmlDocument();
	xmlDoc.Load(gameListPath);
	var gameListElement = xmlDoc.DocumentElement;
	var gameXmlElement = CreateGameXmlElement(xmlDoc, gameListEntry.name, gameListEntry.fileName, gameListEntry.path, gameListEntry.desc, gameListEntry.releasedate, gameListEntry.image, gameListEntry.marquee);
	gameListElement.AppendChild(gameXmlElement);
	xmlDoc.Save(gameListPath);
}

async Task DownloadImage(string url, string path)
{
	var client = new HttpClient();
	var response = await client.GetAsync(url);
	var imageData = await response.Content.ReadAsByteArrayAsync();
	using var stream = new FileStream(path, FileMode.Create);
	stream.Write(imageData, 0, imageData.Length);
}

// You can define other methods, fields, classes and namespaces here
XmlElement CreateGameXmlElement(XmlDocument xmlDoc, string name, string fileName, string path, string desc, string releaseDate, string image, string marquee)
{
	var gameElement = xmlDoc.CreateElement("game");
	
	var pathElement = xmlDoc.CreateElement("path");
	var nameElement = xmlDoc.CreateElement("name");
	var descElement = xmlDoc.CreateElement("desc");
	var dateElement = xmlDoc.CreateElement("releasedate");
	var imageElement = xmlDoc.CreateElement("image");
	var marqueeElement = xmlDoc.CreateElement("marquee");
	
	pathElement.InnerText = path;
	nameElement.InnerText = name;
	descElement.InnerText = desc;
	dateElement.InnerText = releaseDate;
	imageElement.InnerText = $"./media/screenshots/{fileName}.jpg";
	marqueeElement.InnerText = $"./media/marquees/{fileName}.jpg";
	
	gameElement.AppendChild(pathElement);
	gameElement.AppendChild(nameElement);
	gameElement.AppendChild(descElement);
	gameElement.AppendChild(dateElement);
	gameElement.AppendChild(imageElement);
	gameElement.AppendChild(marqueeElement);
	
	return gameElement;
}

string ReplaceInvalidFileChars(string fileName)
{
	var regexSearch = new string(Path.GetInvalidFileNameChars());
	var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
	return Regex.Replace(r.Replace(fileName, ""), @"[^\u0000-\u007F]+", string.Empty);
}
