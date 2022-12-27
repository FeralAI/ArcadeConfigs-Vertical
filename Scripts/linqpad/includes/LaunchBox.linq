<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string LBGamelistTemplate = @"<?xml version=""1.0"" standalone=""yes""?>
<LaunchBox>

</LaunchBox>";

public class LaunchBoxGame
{
	public string PXP
	{
		get
		{
			if (string.IsNullOrWhiteSpace(ApplicationPath))
				return null;

			return Path.GetFileNameWithoutExtension(ApplicationPath);
		}
	}
	public string Title { get; set; }
	public string ApplicationPath { get; set; }
	public string Notes { get; set; }
	public string Developer { get; set; }
	public string Publisher { get; set; }
	public string ReleaseDate { get; set; }
	public double StarRating { get; set; }
}

public void UpdateGame(XmlReader reader, LaunchBoxGame game)
{
	var prop = typeof(LaunchBoxGame)
		.GetProperties()
		.FirstOrDefault(p => p.Name == reader.Name);
		
	if (prop == null)
		return;
	
	try
	{
		var content = reader.ReadElementContentAsString();
		if (!string.IsNullOrWhiteSpace(content))
			prop.SetValue(game, Convert.ChangeType(content, prop.PropertyType));
	}
	catch (Exception ex)
	{
		ex.Dump();
	}
}

public async Task<List<LaunchBoxGame>> LoadLaunchBoxGamelist(string path)
{
	var settings = new XmlReaderSettings { Async = true };
	var reader = XmlReader.Create(path, settings);
	var gameList = new List<LaunchBoxGame>();
	var game = new LaunchBoxGame();
	var inGame = false;

	while (await reader.ReadAsync())
	{
		switch (reader.NodeType)
		{
			case XmlNodeType.Element:
				if (reader.Name == "Game")
				{
					game = new LaunchBoxGame();
					inGame = true;
				}
				else if (inGame) UpdateGame(reader, game);
				break;
			case XmlNodeType.EndElement:
				if (reader.Name == "Game")
				{
					gameList.Add(game);
					inGame = false;
				}
				break;
			default:
				break;
		}
	}
	
	return gameList.OrderBy(g => g.PXP).ToList();
}
