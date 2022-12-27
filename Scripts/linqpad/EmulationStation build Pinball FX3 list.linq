<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

#load "includes\EmulationStation"
#load "includes\LaunchBox"
#load "includes\FileUtils"
#load "includes\HttpUtils"

const string dataSourcePath = @"C:\Temp\Pinball FX3\Pinball FX3.xml";
const string romPath = @"C:\retrobat\roms\pinballfx3\";
const string gameListPath = @"C:\retrobat\roms\pinballfx3\gamelist.xml";
const string mediaPath = @"C:\retrobat\roms\pinballfx3\media";

const string coverPath = @$"{mediaPath}\covers";
const string fanartPath = @$"{mediaPath}\fanart";
const string imagePath = @$"{mediaPath}\images";
const string marqueePath = @$"{mediaPath}\marquees";
const string screenshotPath = @$"{mediaPath}\thumbnails";
const string titlePath = @$"{mediaPath}\screenshottitle";
const string videosPath = @$"{mediaPath}\videos";
const string wheelPath = @$"{mediaPath}\wheel";

async Task Main()
{
	var dataSource = await LoadLaunchBoxGamelist(dataSourcePath);
	//dataSource.Dump();
	
	if (File.Exists(gameListPath))
		File.Delete(gameListPath);
	File.WriteAllText(gameListPath, ESGamelistTemplate);
	
	var pxpPaths = Directory.GetFiles(romPath, "*.pxp");
	//pxpPaths.Dump();
	
	var coverDirInfo = new DirectoryInfo(coverPath).EnumerateFiles();
	var fanartDirInfo = new DirectoryInfo(fanartPath).EnumerateFiles();
	var imageDirInfo = new DirectoryInfo(imagePath).EnumerateFiles();
	var marqueeDirInfo = new DirectoryInfo(marqueePath).EnumerateFiles();
	var screenshotsDirInfo = new DirectoryInfo(screenshotPath).EnumerateFiles();
	var titlesDirInfo = new DirectoryInfo(titlePath).EnumerateFiles();
	var videosDirInfo = new DirectoryInfo(videosPath).EnumerateFiles();
	var wheelDirInfo = new DirectoryInfo(wheelPath).EnumerateFiles();

	foreach (var pxpPath in pxpPaths)
	{
		// Create gamelist entry
		var pxp = Path.GetFileNameWithoutExtension(pxpPath);
		var pxpFilename = Path.GetFileName(pxpPath);
		var launchboxGame = dataSource.FirstOrDefault(g => g.PXP == pxp);
		var coverFile = coverDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var fanartFile = fanartDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var imageFile = imageDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var marqueeFile = marqueeDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var screenshotFile = screenshotsDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var titleFile = titlesDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var videoFile = videosDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		var wheelFile = wheelDirInfo.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == pxp);
		
		var gameListEntry = new GamelistGame
		{
			path = $"./{pxpFilename}",
			fileName = pxp,
			name = launchboxGame.Title,
			desc = launchboxGame.Notes,
			releasedate = DateTime.Parse(launchboxGame.ReleaseDate).ToString("yyyyMMddT000000"),
			developer = launchboxGame.Developer,
			publisher = launchboxGame.Publisher,
			//cover = coverFile.Exists ? $"./media/covers/{coverFile.Name}" : "",
			//fanart = fanartFile.Exists ? $"./media/fanart/{fanartFile.Name}" : "",
			//image = coverFile.Exists ? $"./media/covers/{coverFile.Name}" : "",
			image = imageFile.Exists ? $"./media/images/{imageFile.Name}" : "",
			marquee = marqueeFile.Exists ? $"./media/marquees/{marqueeFile.Name}" : "",
			//marquee = coverFile.Exists ? $"./media/covers/{coverFile.Name}" : "",
			thumbnail = screenshotFile.Exists ? $"./media/thumbnails/{screenshotFile.Name}" : "",
			//screenshottitle = titleFile.Exists ? $"./media/screenshottitle/{titleFile.Name}" : "",
			wheel = wheelFile.Exists ? $"./media/wheel/{wheelFile.Name}" : "",
			video = videoFile.Exists ? $"./media/videos/{videoFile.Name}" : "",
			rating = Math.Round(launchboxGame.StarRating / 5, 2).ToString("F2"), // decimal value 0-1
		};

		var xmlDoc = new XmlDocument();
		xmlDoc.Load(gameListPath);
		CreateGameXmlElement(xmlDoc, gameListEntry).Dump();
		xmlDoc.Save(gameListPath);
	}
}
