<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var datFile = @"C:\Utilities\Romcenter\Datafiles\MAME 0.246.xml";
	var settings = new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Parse  };
	var reader = XmlReader.Create(datFile, settings);
	var list = new List<ArcadeDisplay>();
	ArcadeDisplay display = null;
	while (await reader.ReadAsync())
	{	
		switch (reader.NodeType)
		{
			case XmlNodeType.Element:
				if (reader.Name == "machine")
				{
					display = new ArcadeDisplay { Name = reader.GetAttribute("name") };
					list.Add(display);
				}
				if (reader.Name == "display")
				{
					display.Width = Convert.ToInt32(reader.GetAttribute("width"));
					display.Height = Convert.ToInt32(reader.GetAttribute("height"));
					display.Refresh = Convert.ToDecimal(reader.GetAttribute("refresh"));
					display.Rotate = Convert.ToInt32(reader.GetAttribute("rotate"));
				}
				break;
			case XmlNodeType.EndElement:
				if (reader.Name == "machine")
				{
					display = null;
				}
				break;
			default:
				break;
		}
	}

	//list.Dump();

	// Get ordered list of refresh rates
	//var refreshRates = list.Select(d => d.Refresh).Distinct().OrderBy(r => r);
	//refreshRates.Dump("Refresh Rates");

	// Get order list of resolutions
	var resolutions = list
		.GroupBy(d => new { d.Width, d.Height })
		.Select(g => new { Count = g.Count(), Width = g.First().Width, Height = g.First().Height, Results = g.ToList() })
		.OrderByDescending(r => r.Count);
		//.OrderBy(r => r.Resolution.Width)
		//.ThenBy(r => r.Resolution.Height);

	resolutions.Dump();
}

public class ArcadeDisplay
{
	public string Name { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public decimal Refresh { get; set; }
	public int Rotate { get; set; }
}
