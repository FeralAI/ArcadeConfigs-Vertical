<Query Kind="Program" />

string ReplaceInvalidFileChars(string fileName)
{
	var regexSearch = new string(Path.GetInvalidFileNameChars());
	var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
	return Regex.Replace(r.Replace(fileName, ""), @"[^\u0000-\u007F]+", string.Empty);
}