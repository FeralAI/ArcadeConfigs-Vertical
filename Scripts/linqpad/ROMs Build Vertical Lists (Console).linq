<Query Kind="Program" />

public record ConsoleList (string Label, string Path, string[] Extensions);
public const string RomPath = @"C:\ROMs";
public static List<ConsoleList> RomDirs = new List<ConsoleList> {
	new ConsoleList("Sega Dreamcast", "dreamcast", new string[] { ".chd" }),
	new ConsoleList("NEC PC Engine", "pcengine", new string[] { ".chd", ".zip" }),
	new ConsoleList("Sony PlayStation 2", "ps2", new string[] { ".chd" }),
	new ConsoleList("Sony PlayStation 3", "ps3", new string[] { ".chd" }),
	new ConsoleList("Sony PlayStation Portable", "psp", new string[] { ".chd" }),
	new ConsoleList("Sony PlayStation", "psx", new string[] { ".chd" }),
	new ConsoleList("Sega Saturn", "saturn", new string[] { ".chd" }),
	new ConsoleList("Nintendo Switch", "switch", new string[] { ".chd" }),
	new ConsoleList("Nintendo Wii", "wii", new string[] { ".chd" }),
	new ConsoleList("Microsoft XBOX Live Arcade", "xbla", new string[] { ".chd" }),
	new ConsoleList("Microsoft XBOX", "xbox", new string[] { ".chd" }),
	new ConsoleList("Microsoft XBOX 360", "xbox360", new string[] { ".chd" }),
};

void Main()
{
	
}

// You can define other methods, fields, classes and namespaces here