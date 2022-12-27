<Query Kind="Program" />

const string ESGamelistTemplate = @"<?xml version=""1.0""?>
<gameList>
	
</gameList>";

public class GamelistGame
{
	public string name { get; set; }
	public string path { get; set; }
	public string desc { get; set; }
	public string releasedate { get; set; }
	public string developer { get; set; }
	public string publisher { get; set; }
	public string image { get; set; }
	public string marquee { get; set; }
	public string cover { get; set; }
	public string fanart { get; set; }
	public string thumbnail { get; set; }
	public string screenshottitle { get; set; }
	public string wheel { get; set; }
	public string video { get; set; }
	public string rating { get; set; }
	
	public string fileName { get; set; }
}

public (double X, double Y) ComputeRelativePoint(int maxWidth, int maxHeight, int x, int y)
{
	x = Math.Max(0, Math.Min(x, maxWidth));
	y = Math.Max(0, Math.Min(y, maxHeight));
	return (x / (double)maxWidth, y / (double)maxHeight);
}

XmlElement CreateGameXmlElement(XmlDocument xmlDoc, GamelistGame game)
{
	var type = typeof(GamelistGame);
	var props = type.GetProperties();
	var elements = new List<XmlElement>();
	var gamelistElement = xmlDoc.DocumentElement;
	var gameElement = xmlDoc.CreateElement("game");

	foreach(var prop in props)
	{
		if (prop.Name == "fileName")
			continue;
		
		var value = Convert.ChangeType(prop.GetValue(game), prop.PropertyType);
		
		// Skip empty or default values
		var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
		if (value == defaultValue)
			continue;
		
		// Add the game
		var element = xmlDoc.CreateElement(prop.Name);
		element.InnerText = (string)value;
		gameElement.AppendChild(element);
	}
	
	gamelistElement.AppendChild(gameElement);
	return gameElement;
}

XmlElement CreateGameXmlElement(XmlDocument xmlDoc, string name, string fileName, string path, string desc, string developer, string publisher, string releaseDate, string image, string marquee, string video, double rating)
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
	var ratingElement = xmlDoc.CreateElement("rating");

	pathElement.InnerText = path;
	nameElement.InnerText = name;
	descElement.InnerText = desc;
	developerElement.InnerText = developer;
	publisherElement.InnerText = publisher;
	dateElement.InnerText = releaseDate;
	imageElement.InnerText = $"./media/screenshots/{fileName}.jpg";
	marqueeElement.InnerText = $"./media/marquees/{fileName}.jpg";
	videoElement.InnerText = $"./media/videos/{fileName}.mp4";
	ratingElement.InnerText = rating.ToString("F2");

	gameElement.AppendChild(pathElement);
	gameElement.AppendChild(nameElement);
	gameElement.AppendChild(descElement);
	gameElement.AppendChild(developerElement);
	gameElement.AppendChild(publisherElement);
	gameElement.AppendChild(dateElement);
	gameElement.AppendChild(imageElement);
	gameElement.AppendChild(marqueeElement);
	gameElement.AppendChild(videoElement);
	gameElement.AppendChild(ratingElement);

	return gameElement;
}
