<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

public async Task DownloadBinary(string url, string path)
{
	var client = new HttpClient();
	var response = await client.GetAsync(url);
	if (response.IsSuccessStatusCode)
	{
		using (var stream = response.Content.ReadAsStream())
		using (var writer = new FileStream(path, FileMode.Create))
			await stream.CopyToAsync(writer);
	}
	else
	{
		"Failed".Dump();
	}
}

public async Task DownloadBinary(string url, string path, string username, string password)
{
	var client = new HttpClient();
	var authenticationString = $"{username}:{password}";
	var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
	client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
	var response = await client.GetAsync(url);
	if (response.IsSuccessStatusCode)
	{
		using (var stream = response.Content.ReadAsStream())
		using (var writer = new FileStream(path, FileMode.Create))
			await stream.CopyToAsync(writer);
	}
}