<Query Kind="Program">
  <Namespace>System.Xml.Serialization</Namespace>
</Query>

void Main()
{
	var tablePath = @"C:\Program Files (x86)\Steam\steamapps\common\Pinball FX3\data\steam";
	foreach (var file in Directory.GetFiles(tablePath, "*.pxp"))
	{
		var fileInfo =new FileInfo(file);
		var name = fileInfo.Name.Replace(".pxp", "");
		Console.WriteLine("<game>");
		Console.WriteLine($"<path>./{fileInfo.Name}</path>");
		Console.WriteLine($"<name>{name}</name>");
		Console.WriteLine($"<image>./media/images/{name}.png</image>");
		Console.WriteLine($"<video>./media/videos/{name}.mp4</video>");
		Console.WriteLine("</game>");
	}
}

