<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const int CustomViewportAspectRatioIndex = 23;
const string RomPath = @"C:\ROMs\mame";
const string ConfigPath = @"C:\Emulators\RetroArch\config\MAME (old)";
const string NewConfigPath = @"C:\Emulators\RetroArch\config\MAME";
const string OverlayPath = @"C:\Emulators\RetroArch\overlays\arcade-vertical";
const string OverlayFileTemplate = @":\overlays\arcade-vertical\{0}.png";
const string OverlayConfigTemplate = @":\overlays\arcade-vertical\{0}.cfg";

async Task Main()
{
	var displayData = await GetDisplayData(ArcadeDisplay.HeightQHD);
	//displayData.Dump();
	var configData = GetConfigData(ConfigPath);
	//configData.Dump();
	var romFiles = Directory.GetFiles(RomPath, "*.zip").Select(f => new FileInfo(f).Name);
	//romFiles.Dump();
	var newConfigData = new List<RetroarchGameConfig>();

	foreach (var romFile in romFiles)
	{
		var name = romFile.Replace(".zip", "");
		// Do we have an existing config?
		RetroarchGameConfig config = null;
		string configFile = Path.Join(ConfigPath, $"{name}.cfg");
		if (File.Exists(configFile))
		{
			// Parse the existing config
			config = new RetroarchGameConfig(name, File.ReadAllLines(configFile));

			// Clear out viewport and aspect ratio values
			config.AspectRatioIndex = null;
			config.AspectRatio = null;
			config.CustomViewportHeight = null;
			config.CustomViewportWidth = null;
			config.CustomViewportX = null;
			config.CustomViewportY = null;
		}
		else
		{
			config = new RetroarchGameConfig(name);
			config.UseRunahead = null;
			config.UseSecondInstance = null;
		}

		// Create/overwrite the config file and enable the overlay
		var overlayFile = string.Format(OverlayFileTemplate, name);
		if (File.Exists(Path.Join(OverlayPath, $"{name}.png")))
		{
			var overlayConfigFile = Path.Join(OverlayPath, $"{name}.cfg");
			if (File.Exists(overlayConfigFile))
				File.Delete(overlayConfigFile);

			var overlayConfigData = new []
			{
				"overlays = 1",
				$"overlay0_overlay = {name}.png",
				"overlay0_full_screen = true",
				"overlay0_descs = 0"
			};

			File.WriteAllLines(overlayConfigFile, overlayConfigData);
			
			config.UseInputOverlay = true;
			config.InputOverlay = string.Format(OverlayConfigTemplate, name);
			config.InputOverlayOpacity = 1;
		}
		
		newConfigData.Add(config);
	}
	
	if (!Directory.Exists(NewConfigPath))
		Directory.CreateDirectory(NewConfigPath);

	foreach (var newConfig in newConfigData)
	{
		var newConfigFile = Path.Join(NewConfigPath, $"{newConfig.Name}.cfg");
		if (File.Exists(newConfigFile))
			File.Delete(newConfigFile);
			
		File.WriteAllLines(newConfigFile, newConfig.Export());
	}
	
//
//	var names = Directory.GetFiles(ConfigPath, "*.cfg")
//		.Select(f => new FileInfo(f).Name.Replace(".cfg", ""))
//		.Where(n => !n.StartsWith("_"));
//		
//	var matches = names.Select(n => displayData.FirstOrDefault(d => d.Name == n));
//	foreach (var match in matches)
//	{
//		var gameConfig = configData.FirstOrDefault(c => c.Name == match.Name);
//		gameConfig.AspectRatio = null;
//		gameConfig.AspectRatioIndex = CustomViewportAspectRatioIndex;
//		gameConfig.CustomViewportHeight = match.Dimensions.Height;
//		gameConfig.CustomViewportWidth = match.Dimensions.Width;
//	}
}

public static List<RetroarchGameConfig> GetConfigData(string configPath)
{
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
	
	return configData;
}

public static async Task<List<ArcadeDisplay>> GetDisplayData(int targetHeight)
{
	var datFile = @"C:\Utilities\clrmamepro\datfiles\FinalBurn Neo (ClrMame Pro XML, Arcade only).dat";
	var settings = new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Parse };
	var reader = XmlReader.Create(datFile, settings);
	var list = new List<ArcadeDisplay>();
	ArcadeDisplay display = null;
	while (await reader.ReadAsync())
	{
		switch (reader.NodeType)
		{
			case XmlNodeType.Element:
				if (reader.Name == "game")
				{
					display = new ArcadeDisplay(reader.GetAttribute("name"), targetHeight);
					list.Add(display);
				}
				if (reader.Name == "video")
				{
					display.Width = Convert.ToInt32(reader.GetAttribute("width"));
					display.Height = Convert.ToInt32(reader.GetAttribute("height"));
					display.AspectX = Convert.ToInt32(reader.GetAttribute("aspectx"));
					display.AspectY = Convert.ToInt32(reader.GetAttribute("aspecty"));
					display.Orientation = reader.GetAttribute("orientation");
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
	
	return list.OrderBy(d => d.Name).ToList();
}

public class IntegerScaledDimensionsSummary : IntegerScaledDimensions
{
	public IntegerScaledDimensionsSummary(int aspectX, int aspectY, int originalWidth, int originalHeight, int maxHeight)
		: base(aspectX, aspectY, originalWidth, originalHeight, maxHeight)
	{
		
	}
	
	public IntegerScaledDimensionsSummary(IntegerScaledDimensions dimensions, int count = 0)
		: base(dimensions.AspectX, dimensions.AspectY, dimensions.OriginalWidth, dimensions.OriginalHeight, dimensions.MaxHeight)
	{
		Count = count;
	}
	
	public int Count { get; set; }
}

public class IntegerScaledDimensions
{
	public IntegerScaledDimensions(int aspectX, int aspectY, int originalWidth, int originalHeight, int maxHeight)
	{
		AspectX        = aspectX;
		AspectY        = aspectY;
		OriginalWidth  = originalWidth;
		OriginalHeight = originalHeight;
		MaxHeight      = maxHeight;
		
		Calculate();
	}
	
	private void Calculate()
	{
		if (OriginalWidth == 0 || OriginalHeight == 0)
			return;
		
		// Calculate maximum integer scaled height
		MultiplierY = MaxHeight / OriginalHeight;
		Height = OriginalHeight * MultiplierY;
		
		// Calculate the target aspect ratio (W / H)
		var aspectRatio = (decimal)AspectX / (decimal)AspectY;
		
		// Ideally our original width will scale evenly to the provided aspect ratio
		var targetWidth = (int)Math.Round(Height * aspectRatio);
		MultiplierX = (int)(targetWidth / OriginalWidth);
		Width = OriginalWidth * MultiplierX;
	}
	
	public int AspectX { get; set; }
	public int AspectY { get; set; }
	public int OriginalWidth { get; set; }
	public int OriginalHeight { get; set; }
	public int MaxHeight { get; set; }

	public int Width { get; private set; }
	public int Height { get; private set; }
	public int MultiplierX { get; private set; }
	public int MultiplierY { get; private set; }
	
	public string IntegerScaledFillPercentage => IntegerScaledFill.ToString("0.0%");
	public string AspectRatioSkewPercentage => AspectRatioSkew.ToString("0.0%");

	public decimal IntegerScaledFill => ((decimal)Height / MaxHeight);
	public decimal IntegerScaledAspectRatio => (Height > 0) ? (decimal)Width / Height : 0;
	public decimal TargetAspectRatio => (AspectY > 0) ? (decimal)AspectX / AspectY : 0;
	public decimal AspectRatioSkew => (TargetAspectRatio > 0) ? (TargetAspectRatio - IntegerScaledAspectRatio) / TargetAspectRatio : 0;
}

public class ArcadeDisplay
{
	public const decimal TargetAspectX = 4;
	public const decimal TargetAspectY = 3;
	
	public const int HeightHD    = 720;
	public const int HeightFHD   = 1080;
	public const int HeightWUXGA = 1200;
	public const int HeightQHD   = 1440;
	public const int HeightQXGA  = 1536;
	public const int Height4K    = 2160;
	public const int Height5K    = 2880;
	public const int Height8K    = 4320;

	public static decimal CalculateAspect4x3(int height) => CalculateAspectWidth(height, 4, 3);
	public static decimal CalculateAspect16x9(int height) => CalculateAspectWidth(height, 16, 9);
	public static decimal CalculateAspect16x10(int height) => CalculateAspectWidth(height, 16, 10);
	public static decimal CalculateAspectWidth(int height, decimal aspectX, decimal aspectY)
	{
		if (aspectY == 0)
			return 0;

		var pixels = height / aspectY;
		return pixels * aspectX;
	}

	public ArcadeDisplay(string name, int maxHeight)
	{
		Name = name;
		MaxHeight = maxHeight;
	}

	public string Name { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int AspectX { get; set; }
	public int AspectY { get; set; }
	public string Orientation { get; set; }
	public int MaxHeight { get; set; }
	public IntegerScaledDimensions Dimensions
	{
		get
		{
			return new IntegerScaledDimensions(AspectX, AspectY, Width, Height, MaxHeight);
		}
	}
}

public class RetroarchGameConfig
{
	public static Dictionary<string, Action<RetroarchGameConfig, string>> ConfigMap = new Dictionary<string, Action<RetroarchGameConfig, string>>
	{
		{ "aspect_ratio_index",           (d, v) => d.AspectRatioIndex = Convert.ToInt32(v) },
		{ "custom_viewport_height",       (d, v) => d.CustomViewportHeight = Convert.ToInt32(v) },
		{ "custom_viewport_width",        (d, v) => d.CustomViewportWidth = Convert.ToInt32(v) },
		{ "custom_viewport_x",            (d, v) => d.CustomViewportX = Convert.ToInt32(v) },
		{ "custom_viewport_y",            (d, v) => d.CustomViewportY = Convert.ToInt32(v) },
		{ "input_overlay_enable",         (d, v) => d.UseInputOverlay = Convert.ToBoolean(v) },
		{ "input_overlay",                (d, v) => d.InputOverlay = v },
		{ "input_overlay_opacity",        (d, v) => d.InputOverlayOpacity = Convert.ToDecimal(v) },
		{ "run_ahead_enabled",            (d, v) => d.UseRunahead = Convert.ToBoolean(v) },
		{ "run_ahead_frames",             (d, v) => d.RunAheadFrames = Convert.ToInt32(v) },
		{ "run_ahead_secondary_instance", (d, v) => d.UseSecondInstance = Convert.ToBoolean(v) },
		{ "video_aspect_ratio",           (d, v) => d.AspectRatio = Convert.ToDecimal(v) },
	};

	public RetroarchGameConfig(string name, string[] configData = null)
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
		if (CustomViewportX.HasValue)
			results.Add($"custom_viewport_x = \"{CustomViewportX}\"");
		if (CustomViewportY.HasValue)
			results.Add($"custom_viewport_y = \"{CustomViewportY}\"");
		if (UseInputOverlay.HasValue)
			results.Add($"input_overlay_enable = \"{UseInputOverlay.ToString().ToLower()}\"");
		if (InputOverlay != null)
			results.Add($"input_overlay = \"{InputOverlay}\"");
		if (InputOverlayOpacity.HasValue)
			results.Add($"input_overlay_opacity = \"{InputOverlayOpacity.Value.ToString("F6")}\"");
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
		if (configData != null)
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
	public int? CustomViewportX { get; set; }
	public int? CustomViewportY { get; set; }
	public bool? UseInputOverlay { get; set; }
	public string InputOverlay { get; set; }
	public decimal? InputOverlayOpacity { get; set; }
}
