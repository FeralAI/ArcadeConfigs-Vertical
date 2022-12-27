<Query Kind="Program" />

record RomInfo(string List, string ROM);

void Main()
{
	var currentROMs = @"C:\arcade\roms\fbneo";
	var newROMs = @"X:\ROMs\fbneo-vertical";
	var altROMs = @"X:\ROMs\fbneo-vertical-alt";
	
	var currentROMFiles = Directory
		.GetFiles(currentROMs)
		.Select(f => new FileInfo(f).Name);
		
	var newROMFiles = Directory
		.GetFiles(newROMs)
		.Select(f => new FileInfo(f).Name);
		
	var currentROMsMissing = newROMFiles
		.Except(currentROMFiles)
		.Select(r => new RomInfo("current", r));
		
	currentROMsMissing.Count().Dump("Missing from current");
	
	var newROMsMissing = currentROMFiles
		.Except(newROMFiles)
		.Select(r => new RomInfo("new", r));
	
	newROMsMissing.Count().Dump("Missing from new");
	
	//var allROMs = currentROMsMissing.Union(newROMsMissing).OrderBy(x => x.ROM);
	//allROMs.Dump();
	
	var results = currentROMsMissing.Select(c => {
		// Match chars until we only have one match
		int index = 0, matchCount = 0;
		int length = c.ROM.Length;
		StringBuilder sb = new StringBuilder();
		IEnumerable<RomInfo> matches = new List<RomInfo>();

		while (matchCount != 1 && index < length)
		{
			sb.Append(c.ROM.Substring(index, 1));
			matches = newROMsMissing.Where(n => n.ROM.StartsWith(sb.ToString()));
			matchCount = matches.Count();
			index++;
		}

		return new { Current = c, Matches = matches, Search = sb.ToString() };
	}).ToList();
	
	var matched = results.Where(r => r.Matches.Count() > 0);
	var unmatched = results.Where(r => r.Matches.Count() == 0);
	var matchedNew = matched.SelectMany(r => r.Matches).Distinct().Select(r => r.ROM);
	var unmatchedNew = newROMsMissing.Where(r => !matchedNew.Contains(r.ROM));
	
	matched.Dump("Matched");
	unmatched.Dump("Unmatched");
	unmatchedNew.Dump("Unmatched New");
}

// You can define other methods, fields, classes and namespaces here