<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>CsvHelper.Fluent</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
  <Namespace>CsvHelper.Delegates</Namespace>
  <Namespace>CsvHelper.Expressions</Namespace>
  <Namespace>CsvHelper.Fluent</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{	
	// Read in the map from PXP to asset name options
	var pxpAssetMap = ImportAssetMap(assetListPath);
	if (pxpAssetMap == null)
	{
		"Missing PXP mapping csv file".Dump();
		return;
	}
	//pxpAssetMap.Dump();

	// Parse name options for matching
	var nameOptions = GenerateNameOptions(pxpAssetMap);
	//nameOptions.Dump();

	// Process asset dirs
	foreach (var matchPath in matchPaths)
	{
		var matchSummary = ProcessAssetDir(matchPath.SourcePath, matchPath.TargetPath, nameOptions);
		matchSummary.Dump(matchPath.Name);
		
		if (!Directory.Exists(matchPath.TargetPath))
			Directory.CreateDirectory(matchPath.TargetPath);

		matchSummary.Matched.ForEach(r => {
			var matchResult = r.MatchResults[0];
			var fileExt = new FileInfo(r.Name).Extension;
			var targetFile = Path.Combine(matchPath.TargetPath, matchResult.PXP + fileExt);
			if (File.Exists(targetFile))
				File.Delete(targetFile);
			File.Copy(Path.Combine(matchPath.SourcePath, r.Name), targetFile);
		});

		/* Show duplicate image matches */
		//matchSummary.Matched
		//	.Select(m => new { Name = m.Name, Match = m.MatchResults.First() })
		//	.GroupBy(m => m.Match.PXP, m => m)
		//	.Where(g => g.Count() > 1)
		//	.OrderBy(g => g.Key)
		//	.Dump(matchPath.Name);
	}
}

public record PXPAssetMap(string PXP, string Box, string Video);
public record PXPNameOptions(string PXP, List<string> NameOptions, List<HashSet<string>> NameOptionsParts);
public record PXPMatchResult(string PXP, double MatchScore, HashSet<string> MatchResult);
public record PXPMatchResults(string Name, List<PXPMatchResult> MatchResults);
public record PXPMatchSummary(List<PXPMatchResults> Matched, List<PXPMatchResults> Unmatched);
public record PXPMatchPaths(string Name, string SourcePath, string TargetPath);

const string assetListPath = @"C:\Temp\Pinball FX3\Pinball FX3 asset list.csv";

const string baseTargetPath = @"C:\Temp\Pinball FX3\media";
const string baseSourcePath = @"C:\Temp\Pinball FX3";
const string baseImageSourcePath = @"C:\Temp\Pinball FX3\images";

const string sourceBGPath = @$"{baseImageSourcePath}\Fanart - Background";
const string sourceBoxPath = @$"{baseImageSourcePath}\Box - Front - Reconstructed";
const string sourceFlyersFrontPath = @$"{baseImageSourcePath}\Advertisement Flyer - Front";
const string sourceFlyersBackPath = @$"{baseImageSourcePath}\Advertisement Flyer - Back";
const string sourceImagesPath = @$"{baseImageSourcePath}\Screenshot - Game Select (corrected)";
const string sourceLogoPath = @$"{baseImageSourcePath}\Clear Logo";
const string sourceScreenshotPath = @$"{baseImageSourcePath}\Screenshot - Gameplay";
const string sourceSteamImagePath = @$"{baseImageSourcePath}\Marquees 1280x540";
const string sourceTitlePath = @$"{baseImageSourcePath}\Screenshot - Game Title";
const string sourceVideoPath = @$"{baseSourcePath}\videos";

const string targetBGPath = @$"{baseTargetPath}\fanart";
const string targetBoxPath = @$"{baseTargetPath}\covers";
const string targetFlyersFrontPath = @$"{baseTargetPath}\flyers";
const string targetFlyersBackPath = @$"{baseTargetPath}\flyers-back";
const string targetImagesPath = @$"{baseTargetPath}\tables";
const string targetLogoPath = @$"{baseTargetPath}\wheel";
const string targetScreenshotPath = @$"{baseTargetPath}\screenshot";
const string targetSteamImagePath = @$"{baseTargetPath}\steamgrid";
const string targetTitlePath = @$"{baseTargetPath}\screenshottitle";
const string targetVideoPath = @$"{baseTargetPath}\videos";

public static List<string> pxpPrefixes = new List<string> { "BALLY", "BETHESDA", "MARVEL", "STARWARS", "UNIVERSAL", "WMS" };
public static List<string> excludePrefixes = new List<string> { "marvel", "star wars" };
public static List<string> excludedChars = new List<string> { ".", "\'", "-" };
public static List<string> spaceChars = new List<string> { "_", "'" };
public static List<string> excludedWords = new List<string> { "of", "the" };

public static List<PXPMatchPaths> matchPaths = new List<PXPMatchPaths>
{
	new PXPMatchPaths("Backgrounds", sourceBGPath, targetBGPath),
	new PXPMatchPaths("Covers", sourceBoxPath, targetBoxPath),
	new PXPMatchPaths("Flyers", sourceFlyersFrontPath, targetFlyersFrontPath),
	new PXPMatchPaths("Flyers Back", sourceFlyersBackPath, targetFlyersBackPath),
	new PXPMatchPaths("Logos", sourceLogoPath, targetLogoPath),
	new PXPMatchPaths("Screenshots", sourceScreenshotPath, targetScreenshotPath),
	new PXPMatchPaths("Images", sourceImagesPath, targetImagesPath),
	new PXPMatchPaths("Steam Images", sourceSteamImagePath, targetSteamImagePath),
	new PXPMatchPaths("Titles", sourceTitlePath, targetTitlePath),
	new PXPMatchPaths("Videos", sourceVideoPath, targetVideoPath),
};

public double GetMatchScore(HashSet<string> searchTerms, HashSet<string> checkTerms)
{
	var matchCount = searchTerms.Count(s => checkTerms.Contains(s));
	return matchCount / (double)Math.Max(searchTerms.Count, checkTerms.Count);
}

public PXPMatchSummary ProcessAssetDir(string sourcePath, string targetPath, List<PXPNameOptions> dataSource)
{
	var matchResults = Directory
		.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly)
		.Select(f => new FileInfo(f))
		.Select(f =>
		{
			var cleanName = CleanName(Path.GetFileNameWithoutExtension(f.FullName));
			var nameParts = SplitName(cleanName);
			return new PXPMatchResults(
				f.Name,
				dataSource.SelectMany(o => 
					o.NameOptionsParts
						.Select(p => new PXPMatchResult(o.PXP, GetMatchScore(nameParts, p), p))
				)
				.Where(r => r.MatchScore == 1)
				.ToList()
			);
		});

	return new PXPMatchSummary
	(
		matchResults.Where(r => r.MatchResults.Count > 0).ToList(),
		matchResults.Where(r => r.MatchResults.Count == 0).ToList()
	);
}

public string CleanName(string name)
{
	// Strip any suffix like "(Bally 1990)"
	name = Regex.Replace(name, @"\s\(.*\)$", ""); 
	
	foreach (var prefix in pxpPrefixes)
		if (name.StartsWith(prefix))
			name = name.Replace($"{prefix}", "");
	
	foreach (var excluded in excludedChars)
		if (name.Contains(excluded))
			name = name.Replace(excluded, "");

	foreach (var spaceChar in spaceChars)
		if (name.Contains(spaceChar))
			name = name.Replace(spaceChar, " ");

	while (name.Contains("  "))
		name = name.Replace("  ", " ");
		
	// Cleanup words that should be "'s"
	if (name.Contains(" s "))
		name = name.Replace(" s", "s");
		
	name = name.Trim();

	return name;
}

public HashSet<string> SplitName(string name)
{
	var loweredName = name
			.Trim()
			.ToLower();

	excludePrefixes.ForEach(p =>
	{
		if (loweredName.StartsWith(p))
			loweredName = loweredName.Replace(p, "").Trim();
	});

	return loweredName
			.Split(' ')
			.Where(s => !excludedWords.Contains(s))
			.ToHashSet();
}

public List<PXPAssetMap> ImportAssetMap(string file)
{
	IEnumerable<PXPAssetMap> pxpAssetMap;
	using (var reader = new StreamReader(assetListPath))
	using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
	{
		try
		{
			pxpAssetMap = csv.GetRecords<PXPAssetMap>();
			return pxpAssetMap.ToList();
		}
		catch { return null; }
	}
}

public List<PXPNameOptions> GenerateNameOptions(List<PXPAssetMap> assetMap)
{
	return assetMap
		.Select(m =>
		{
			var nameOptions = new List<string>
			{
				CleanName(m.PXP),
				CleanName(m.Box),
				CleanName(m.Video),
			}
			.Distinct();

			var nameOptionsParts = nameOptions
				.Select(SplitName)
				.Distinct(HashSet<string>.CreateSetComparer());
			
			return new PXPNameOptions
			(
				m.PXP,
				nameOptions.ToList(),
				nameOptionsParts.ToList()
			);
		})
		.ToList();
}



