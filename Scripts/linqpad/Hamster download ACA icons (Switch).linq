<Query Kind="Statements">
  <NuGetReference>AngleSharp</NuGetReference>
  <Namespace>AngleSharp</Namespace>
  <Namespace>AngleSharp.Attributes</Namespace>
  <Namespace>AngleSharp.Browser</Namespace>
  <Namespace>AngleSharp.Browser.Dom</Namespace>
  <Namespace>AngleSharp.Browser.Dom.Events</Namespace>
  <Namespace>AngleSharp.Common</Namespace>
  <Namespace>AngleSharp.Css</Namespace>
  <Namespace>AngleSharp.Css.Dom</Namespace>
  <Namespace>AngleSharp.Css.Parser</Namespace>
  <Namespace>AngleSharp.Dom</Namespace>
  <Namespace>AngleSharp.Dom.Events</Namespace>
  <Namespace>AngleSharp.Html</Namespace>
  <Namespace>AngleSharp.Html.Dom</Namespace>
  <Namespace>AngleSharp.Html.Dom.Events</Namespace>
  <Namespace>AngleSharp.Html.Forms</Namespace>
  <Namespace>AngleSharp.Html.Forms.Submitters</Namespace>
  <Namespace>AngleSharp.Html.InputTypes</Namespace>
  <Namespace>AngleSharp.Html.LinkRels</Namespace>
  <Namespace>AngleSharp.Html.Parser</Namespace>
  <Namespace>AngleSharp.Html.Parser.Tokens</Namespace>
  <Namespace>AngleSharp.Io</Namespace>
  <Namespace>AngleSharp.Io.Dom</Namespace>
  <Namespace>AngleSharp.Io.Processors</Namespace>
  <Namespace>AngleSharp.Mathml.Dom</Namespace>
  <Namespace>AngleSharp.Media</Namespace>
  <Namespace>AngleSharp.Media.Dom</Namespace>
  <Namespace>AngleSharp.Scripting</Namespace>
  <Namespace>AngleSharp.Svg.Dom</Namespace>
  <Namespace>AngleSharp.Text</Namespace>
  <Namespace>AngleSharp.Xhtml</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "includes\HttpUtils"

var romPath = @"C:\retrobat\roms\switch-aa\";
var games = Directory.GetFiles(romPath, "ACA NEOGEO *.xci")
	.Select(p => Path.GetFileNameWithoutExtension(p))
	.Select(f => new
	{
		FullName = f,
		SaveName = f.Substring(0, f.IndexOf("[") - 1).Trim(),
		Name = f
			.Substring(0, f.IndexOf("[") - 1)
			.Replace("ACA NEOGEO ", "")
			.Replace(" & ", "and")
			.Trim()
	})
	.ToList();

var downloadPath = @"C:\retrobat\roms\switch-aa\media\images-aca\";
if (Directory.Exists(downloadPath))
	Directory.Delete(downloadPath, true);
Directory.CreateDirectory(downloadPath);

// {0} should be the lowered camelcase name of the game:
// Pac-mania = "pac-mania"
var hqIconBaseUrl = "http://www.hamster.co.jp/american_hamster/arcadearchives/switch/images/title/{0}/{0}_01.jpg";

// Need to parse the HTML on the page
var titleListUrl = "http://www.hamster.co.jp/american_hamster/arcadearchives/switch/title_list_neogeo.htm";

var httpClient = new HttpClient();
var response = await httpClient.GetAsync(titleListUrl);
if (!response.IsSuccessStatusCode)
	return;

var html = await response.Content.ReadAsStringAsync();

var parser = new HtmlParser();
var document = await parser.ParseDocumentAsync(html);

//var qs = $"img[alt=\"{g}\"]";
var qs = ".titleName img";
var imgs = document
	.QuerySelectorAll<IHtmlImageElement>(qs)
	.ToList();

games.ForEach(async g =>
{
	var img = imgs.FirstOrDefault(i => i.AlternativeText == g.Name);

	var imgSlug = img.Attributes["src"]
		.TextContent
		.Split('/')
		.Last()
		.Replace("icon_", "")
		.Replace(".jpg", "");

	var fileName = Path.Combine(downloadPath, $"{g.SaveName}.jpg");
	var imgUrl = string.Format(hqIconBaseUrl, imgSlug);
	await DownloadBinary(imgUrl, fileName);
});
