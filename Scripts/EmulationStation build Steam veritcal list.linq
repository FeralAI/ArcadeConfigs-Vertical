<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Web</Namespace>
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
		378770,  // ESCHATOS
		577910,  // Ghost_Blade_HD
		253750,  // Ikaruga
		278510,  // JUDGEMENT_SILVERSWORD__Resurrection
		463070,  // Mecha_Ritz_Steel_Rondo
		377860,  // Mushihimesama
		442120,  // Pinball_FX3
		323460,  // Raiden_IV_OverKill
		1133300, // Sisters_Royale_Five_Sisters_Under_Fire
		1085630, // VASARA_Collection
	};
	
	var imagePath = @"C:\ROMs\steam\media";
	var appDetailsPath = "/api/appdetails/?appids=";
	
	foreach (var appId in appIds)
	{
		var client = new HttpClient();
		client.BaseAddress = new Uri("https://store.steampowered.com");
		var response = await client.GetAsync(appDetailsPath + appId);
		var result = await response.Content.ReadAsStringAsync();
		var jsonDoc = JsonDocument.Parse(result);
		var jsonRoot = jsonDoc.RootElement;
		var gameElement = jsonRoot.GetProperty(appId.ToString());
		var gameDataElement = gameElement.GetProperty("data");

		var gameAppId = gameDataElement.GetProperty("steam_appid").GetInt32();
		var developers = new List<string>();
		foreach (var developer in gameDataElement.GetProperty("developers").EnumerateArray())
			developers.Add(developer.GetString());

		var publishers = new List<string>();
		foreach (var publisher in gameDataElement.GetProperty("publishers").EnumerateArray())
			publishers.Add(publisher.GetString());
		
		var gameListEntry = new
		{
			path = $"./{gameAppId}.sid",
			name = gameDataElement.GetProperty("name").GetString(),
			desc = HttpUtility.HtmlDecode(gameDataElement.GetProperty("short_description").GetString()),
			releasedate = DateTime.Parse(gameDataElement.GetProperty("release_date").GetProperty("date").GetString()).ToString("yyyyMMddT000000"),
			developer = string.Join(" / ", developers),
			publisher = string.Join(" / ", publishers),
			image = gameDataElement.GetProperty("screenshots").EnumerateArray().First(_ => _.GetProperty("path_full").GetString().Contains(".jpg")).GetProperty("path_full").GetString(),
			marquee = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameAppId}/header.jpg",
			fileName = ReplaceInvalidFileChars(gameDataElement.GetProperty("name").GetString()),
			movie = gameDataElement.GetProperty("movies").EnumerateArray().First().GetProperty("mp4").GetProperty("480").GetString(),
			genre = gameDataElement.GetProperty("genres").EnumerateArray().First().GetProperty("description").GetString(),
		};

		gameListEntry.Dump();

		// Download the images
		await DownloadBinary(gameListEntry.image, @$"{imagePath}\screenshots\{gameListEntry.fileName}.jpg");
		await DownloadBinary(gameListEntry.marquee, @$"{imagePath}\marquees\{gameListEntry.fileName}.jpg");
		await DownloadBinary(gameListEntry.movie, @$"{imagePath}\videos\{gameListEntry.fileName}.mp4");

		// Create SID file
		var sidFile = @$"C:\ROMs\steam\{gameAppId}.sid";
		if (!File.Exists(sidFile))
			File.Create(sidFile);

		// Update game list
		var gameListPath = @"C:\ROMs\steam\gamelist.xml";
		if (!File.Exists(gameListPath))
			File.Create(gameListPath);
			
		var xmlDoc = new XmlDocument();
		xmlDoc.Load(gameListPath);
		var gameListElement = xmlDoc.DocumentElement;
		var gameXmlElement = CreateGameXmlElement(xmlDoc, gameListEntry.name, gameListEntry.fileName, gameListEntry.path, gameListEntry.desc, gameListEntry.developer, gameListEntry.publisher, gameListEntry.releasedate, gameListEntry.image, gameListEntry.marquee, gameListEntry.movie);
		gameListElement.AppendChild(gameXmlElement);
		xmlDoc.Save(gameListPath);
	}
}

async Task DownloadBinary(string url, string path)
{
	var client = new HttpClient();
	var response = await client.GetAsync(url);
	var imageData = await response.Content.ReadAsByteArrayAsync();
	using var stream = new FileStream(path, FileMode.Create);
	stream.Write(imageData, 0, imageData.Length);
}

// You can define other methods, fields, classes and namespaces here
XmlElement CreateGameXmlElement(XmlDocument xmlDoc, string name, string fileName, string path, string desc, string developer, string publisher, string releaseDate, string image, string marquee, string video)
{
	var gameElement = xmlDoc.CreateElement("game");
	
	var pathElement = xmlDoc.CreateElement("path");
	var nameElement = xmlDoc.CreateElement("name");
	var descElement = xmlDoc.CreateElement("desc");
	var developerElement = xmlDoc.CreateElement("developer");
	var publisherElement = xmlDoc.CreateElement("publisher");
	var dateElement = xmlDoc.CreateElement("releasedate");
	var imageElement = xmlDoc.CreateElement("image");
	var marqueeElement = xmlDoc.CreateElement("marquee");
	var videoElement = xmlDoc.CreateElement("video");
	
	pathElement.InnerText = path;
	nameElement.InnerText = name;
	descElement.InnerText = desc;
	developerElement.InnerText = developer;
	publisherElement.InnerText = publisher;
	dateElement.InnerText = releaseDate;
	imageElement.InnerText = $"./media/screenshots/{fileName}.jpg";
	marqueeElement.InnerText = $"./media/marquees/{fileName}.jpg";
	videoElement.InnerText = $"./media/videos/{fileName}.mp4";
	
	gameElement.AppendChild(pathElement);
	gameElement.AppendChild(nameElement);
	gameElement.AppendChild(descElement);
	gameElement.AppendChild(developerElement);
	gameElement.AppendChild(publisherElement);
	gameElement.AppendChild(dateElement);
	gameElement.AppendChild(imageElement);
	gameElement.AppendChild(marqueeElement);
	gameElement.AppendChild(videoElement);
	
	return gameElement;
}

string ReplaceInvalidFileChars(string fileName)
{
	var regexSearch = new string(Path.GetInvalidFileNameChars());
	var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
	return Regex.Replace(r.Replace(fileName, ""), @"[^\u0000-\u007F]+", string.Empty);
}
