<Query Kind="Program" />

const int Width = 1080;
const int Height = 1920;
const int Precision = 3;

void Main()
{
	var positions = new List<(int X, int Y)>
	{
		// Enter X and Y positions via console
		//(int.Parse(Util.ReadLine("Enter X position")), int.Parse(Util.ReadLine("Enter Y position"))),
		
		// Define a list of predefined positions to calculate
		(Width / 2, Height / 2),
		(100, 150),
	};
	
	var format = "0." + new String('#', Precision);

	foreach (var position in positions)
	{
		var relative = ComputeRelativePoint(Width, Height, position.X, position.Y);
		string.Format(
			"({0}, {1}) = {2} {3}",
			position.X,
			position.Y,
			relative.X.ToString(format),
			relative.Y.ToString(format)
		).Dump();
	}
}

public (double X, double Y) ComputeRelativePoint(int maxWidth, int maxHeight, int x, int y)
{
	x = Math.Max(0, Math.Min(x, maxWidth));
	y = Math.Max(0, Math.Min(y, maxHeight));
	return (x / (double)maxWidth, y / (double)maxHeight);
}