<Query Kind="Program">
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>

void Main()
{
	var configPath = @"C:\Emulators\RetroArch\config\FinalBurn Neo\";
	var configData = new List<RetroarchGameConfig>();
	
	foreach (var file in Directory.GetFiles(configPath, "*.cfg"))
	{
		var info = new FileInfo(file);
		
		// Ignore underscore files
		if (info.Name.StartsWith("_"))
			continue;

		var data = new RetroarchGameConfig(info.Name.Replace(".cfg", ""), File.ReadAllLines(file));
		configData.Add(data);
	}
	
	configData.Count.Dump();
	configData.OrderBy(d => d.Name).Dump();
	
	configData.ForEach(d => d.Export().Dump(d.Name));
}

public class RetroarchGameConfig
{
	public static Dictionary<string, Action<RetroarchGameConfig, string>> ConfigMap = new Dictionary<string, Action<RetroarchGameConfig, string>>
	{
		{ "aspect_ratio_index",           (d, v) => d.AspectRatioIndex = Convert.ToInt32(v) },
		{ "custom_viewport_height",       (d, v) => d.CustomViewportHeight = Convert.ToInt32(v) },
		{ "custom_viewport_width",        (d, v) => d.CustomViewportWidth = Convert.ToInt32(v) },
		{ "run_ahead_enabled",            (d, v) => d.UseRunahead = Convert.ToBoolean(v) },
		{ "run_ahead_frames",             (d, v) => d.RunAheadFrames = Convert.ToInt32(v) },
		{ "run_ahead_secondary_instance", (d, v) => d.UseSecondInstance = Convert.ToBoolean(v) },
		{ "video_aspect_ratio",           (d, v) => d.AspectRatio = Convert.ToDecimal(v) },
	};

	public RetroarchGameConfig() {}
	public RetroarchGameConfig(string name, string[] configData)
	{
		Name = name;
		Parse(configData);
	}
	
	public List<string> Export()
	{
		var results = new List<string>();
		if (AspectRatioIndex.HasValue)
			results.Add($"aspect_ratio_index = \"{AspectRatioIndex}\"");
		if (CustomViewportHeight.HasValue)
			results.Add($"custom_viewport_height = \"{CustomViewportHeight}\"");
		if (CustomViewportWidth.HasValue)
			results.Add($"custom_viewport_width = \"{CustomViewportWidth}\"");
		if (UseRunahead.HasValue)
			results.Add($"run_ahead_enabled = \"{UseRunahead.ToString().ToLower()}\"");
		if (RunAheadFrames.HasValue)
			results.Add($"run_ahead_frames = \"{RunAheadFrames}\"");
		if (UseSecondInstance.HasValue)
			results.Add($"run_ahead_secondary_instance = \"{UseSecondInstance.ToString().ToLower()}\"");
		if (AspectRatio.HasValue)
			results.Add($"video_aspect_ratio = \"{AspectRatio}\"");

		return results;
	}
	
	public void Parse(string[] configData)
	{
		foreach (var line in configData)
		{
			if (string.IsNullOrWhiteSpace(line))
				continue;

			var parts = line.Split(" = ");
			if (parts.Length != 2)
				continue;

			if (!ConfigMap.ContainsKey(parts[0]))
				continue;

			var value = parts[1].Substring(1, parts[1].Length - 2);
			ConfigMap[parts[0]](this, value);
		}
		
		// Default to 1 frame if run ahead enabled but no frame value
		if (UseRunahead == true && !RunAheadFrames.HasValue)
			RunAheadFrames = 1;
		
		// Force no second instance for FBNeo games
		UseSecondInstance = false;
		
		if (UseRunahead != true)
			UseRunahead = false;
	}

	public string Name { get; set; }
	public bool? UseRunahead { get; set; }
	public int? RunAheadFrames { get; set; }
	public bool? UseSecondInstance { get; set; }
	public int? AspectRatioIndex { get; set; }
	public decimal? AspectRatio { get; set; }
	public int? CustomViewportWidth { get; set; }
	public int? CustomViewportHeight { get; set; }
}