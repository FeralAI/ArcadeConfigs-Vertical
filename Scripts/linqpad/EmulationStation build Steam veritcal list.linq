<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

#load "includes\EmulationStation"
#load "includes\FileUtils"
#load "includes\HttpUtils"

const string SteamCmdTemplate = @"@echo OFF
start steam://rungameid/{0}
timeout /t 1
:RUNNING

tasklist|findstr ""{1}.exe"" > nul

if %errorlevel%==1 timeout /t 5 & GOTO ENDLOOP
timeout /t 2
GOTO RUNNING
:ENDLOOP";

public record SteamCmdInfo(int AppId, string ExeName);

async Task Main()
{
	var apps = new List<SteamCmdInfo>
	{
		new SteamCmdInfo(403400, "DIG DUG"),  // ARCADE_GAME_SERIES_DIG_DUG
		new SteamCmdInfo(403430, "GALAGA"),  // ARCADE_GAME_SERIES_GALAGA
		new SteamCmdInfo(394160, "PAC-MAN"),  // ARCADE_GAME_SERIES_PACMAN
		new SteamCmdInfo(403410, "Ms. PAC-MAN"),  // ARCADE_GAME_SERIES_Ms_PACMAN
		new SteamCmdInfo(489580, "AWA"),  // AWA
		new SteamCmdInfo(439490, "bluerevolver"),  // BLUE_REVOLVER
		new SteamCmdInfo(545060, "BulletSoulIBWin"),  // BULLET_SOUL_INFINITE_BURST
		new SteamCmdInfo(1515950, "CapcomArcadeStadium"),  // Capcom_Arcade_Stadium
		new SteamCmdInfo(1755910, "CapcomArcade2ndStadium"),  // Capcom_Arcade_2nd_Stadium
		new SteamCmdInfo(582980, "Shikigami"),  // Castle of Shikigami
		new SteamCmdInfo(1556650, "Shiki2"), // Castle of Shikigami 2
		new SteamCmdInfo(1718160, "CrimzonCloverWEX"), // Crimzon_Clover_World_EXplosion
		new SteamCmdInfo(280560, "DU2"),  // Danmaku_Unlimited_2
		new SteamCmdInfo(450950, "DU3"),  // Danmaku_Unlimited_3
		new SteamCmdInfo(379520, "deltazeal"),  // DELTAZEAL
		new SteamCmdInfo(422510, "DEMON'S TILT"),  // DEMONS_TILT
		new SteamCmdInfo(464450, "default"),  // DoDonPachi_Resurrection
		new SteamCmdInfo(360740, "Downwell"),  // Downwell
		new SteamCmdInfo(1279420, "dragonblaze"),  // Dragon_Blaze
		new SteamCmdInfo(378770, "ESCHATOS"),  // ESCHATOS
		new SteamCmdInfo(465060, "ExZeal"),  // EXZEAL
		new SteamCmdInfo(509780, "FAX"),  // Fire_Arrow_Plus
		new SteamCmdInfo(663130, "Game Tengoku CruisinMix"),  // Game_Tengoku_CruisinMix_Special
		new SteamCmdInfo(577910, "GhostBlade"),  // Ghost_Blade_HD
		new SteamCmdInfo(1342240, "Gunbarich"),  // GUNBARICH
		new SteamCmdInfo(1261970, "gunbird"),  // GUNBIRD
		new SteamCmdInfo(1279410, "gunbird2"),  // GUNBIRD_2
		new SteamCmdInfo(2025840, "gunvein"), // Gunvein
		new SteamCmdInfo(1696200, "HellBlasters"),  // Hell_Blasters
		new SteamCmdInfo(1071260, "HorizonShift81"), // Horizon_Shift_81
		new SteamCmdInfo(375000, "Icarus-X-ToF"),  // IcarusX_Tides_of_Fire
		new SteamCmdInfo(253750, "game"),  // Ikaruga
		new SteamCmdInfo(278510, "JSSResurrection"),  // JUDGEMENT_SILVERSWORD__Resurrection
		new SteamCmdInfo(283820, "Kamui"),  // KAMUI
		new SteamCmdInfo(463070, "MECHA_SR"),  // Mecha_Ritz_Steel_Rondo
		new SteamCmdInfo(430300, "mz"),  // MINUS_ZERO
		new SteamCmdInfo(603960, "Monolith"),  // Monolith
		new SteamCmdInfo(1790250, "MoonDancer"),  // Moon_Dancer
		new SteamCmdInfo(377860, "default"),  // Mushihimesama
		new SteamCmdInfo(442120, "Pinball FX3"),  // Pinball_FX3
		new SteamCmdInfo(998990, "PsyvariarDelta"),  // Psyvariar_Delta
		new SteamCmdInfo(315670, "Raiden3"),  // Raiden_III_Digital_Edition
		new SteamCmdInfo(323460, "game"),  // Raiden_IV_OverKill
		new SteamCmdInfo(407600, "raidenlegacy"),  // Raiden_Legacy__Steam_Edition
		new SteamCmdInfo(1261980, "saces"),  // Samurai_Aces
		new SteamCmdInfo(430290, "sst"),  // Shmups_Skill_Test
		new SteamCmdInfo(1133300, "SistersRoyale"), // Sisters_Royale_Five_Sisters_Under_Fire
		new SteamCmdInfo(1410440, "Sophstar"),  // Sophstar
		new SteamCmdInfo(1570500, "Space Moth - Lunar Edition"), // Space_Moth_Lunar_Edition
		new SteamCmdInfo(1261960, "s1945"),  // STRIKERS_1945
		new SteamCmdInfo(1279380, "s1945ii"),  // STRIKERS_1945_II
		new SteamCmdInfo(1279400, "s1945iii"),  // STRIKERS_1945_III
		new SteamCmdInfo(271860, "SKHR"),  // Super_Killer_Hornet_Resurrection
		new SteamCmdInfo(498470, "Switch 'N' Shoot"),  // Switch_N_Shoot
		new SteamCmdInfo(465070, "TriZeal"),  // TRIZEAL_Remix
		new SteamCmdInfo(1085630, "VASARA Collection"), // VASARA_Collection
		new SteamCmdInfo(419090, "VectorStrain"),  // Vector_Strain
		new SteamCmdInfo(364250, "xiizeal"),  // XIIZEAL
		new SteamCmdInfo(448160, "WOLFLAME"),  // WOLFLAME
		new SteamCmdInfo(466820, "Zenodyne_Remake"),  // Zenodyne_R
		new SteamCmdInfo(402030, "Zenohell"),  // Zenohell
		new SteamCmdInfo(809020, "ZeroRanger"),  // ZeroRanger
	};

	var imagePath = @"C:\retrobat\roms\steam\media";
	var appDetailsPath = "/api/appdetails/?appids=";
	var appReviewsPath = "/appreviews/{0}?json=1";
	var gameListPath = @"C:\retrobat\roms\steam\gamelist.xml";
	
	if (File.Exists(gameListPath))
		File.Delete(gameListPath);
	File.WriteAllText(gameListPath, ESGamelistTemplate);
	
	foreach (var app in apps)
	{
		// Create CMD file
		var cmdFile = @$"C:\retrobat\roms\steam\{app.AppId}.cmd";
		if (File.Exists(cmdFile))
			File.Delete(cmdFile);
		File.WriteAllText(cmdFile, string.Format(SteamCmdTemplate, app.AppId, app.ExeName));

		var client = new HttpClient();
		client.BaseAddress = new Uri("https://store.steampowered.com");
		
		// Get app data
		var response = await client.GetAsync(appDetailsPath + app.AppId);
		if (!response.IsSuccessStatusCode)
			continue;

		var result = await response.Content.ReadAsStringAsync();
		var jsonDoc = JsonDocument.Parse(result);
		var jsonRoot = jsonDoc.RootElement;
		
		var gameElement = jsonRoot.GetProperty(app.AppId.ToString());
		var gameDataElement = gameElement.GetProperty("data");

		var gameAppId = gameDataElement.GetProperty("steam_appid").GetInt32();
		var developers = new List<string>();
		foreach (var developer in gameDataElement.GetProperty("developers").EnumerateArray())
			developers.Add(developer.GetString());

		var publishers = new List<string>();
		foreach (var publisher in gameDataElement.GetProperty("publishers").EnumerateArray())
			publishers.Add(publisher.GetString());

		// Get review data
		response = await client.GetAsync(string.Format(appReviewsPath, app.AppId));
		result = await response.Content.ReadAsStringAsync();
		jsonDoc = JsonDocument.Parse(result);
		jsonRoot = jsonDoc.RootElement;
		var ratingSummary = jsonRoot.GetProperty("query_summary");
		var totalReviewCount = ratingSummary.GetProperty("total_reviews");
		var positiveReviewCount = ratingSummary.GetProperty("total_positive");
		var rating = positiveReviewCount.GetInt32() / totalReviewCount.GetDouble();

		// Create gamelist entry
		var gameListEntry = new
		{
			path = $"./{gameAppId}.cmd",
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
			rating = Math.Round(rating, 2), // decimal value 0-1
		};

		gameListEntry.Dump();

		// Download assets
		await DownloadBinary(gameListEntry.image, @$"{imagePath}\screenshots\{gameListEntry.fileName}.jpg");
		await DownloadBinary(gameListEntry.marquee, @$"{imagePath}\marquees\{gameListEntry.fileName}.jpg");
		await DownloadBinary(gameListEntry.movie, @$"{imagePath}\videos\{gameListEntry.fileName}.mp4");
			
		var xmlDoc = new XmlDocument();
		xmlDoc.Load(gameListPath);
		var gameListElement = xmlDoc.DocumentElement;
		var gameXmlElement = CreateGameXmlElement(xmlDoc, gameListEntry.name, gameListEntry.fileName, gameListEntry.path, gameListEntry.desc, gameListEntry.developer, gameListEntry.publisher, gameListEntry.releasedate, gameListEntry.image, gameListEntry.marquee, gameListEntry.movie, gameListEntry.rating);
		gameListElement.AppendChild(gameXmlElement);
		xmlDoc.Save(gameListPath);
	}
}
