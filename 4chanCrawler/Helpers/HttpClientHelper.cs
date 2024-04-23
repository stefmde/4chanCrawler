namespace _4chanCrawler.Helpers;

public class HttpClientHelper
{
	public async Task<string?> GetJson(string url)
	{
		var client = new HttpClient();
		try
		{
			var clientResult = await client.GetAsync(url);
			if (!clientResult.IsSuccessStatusCode)
			{
				return null;
			}

			var resultJson = await clientResult.Content.ReadAsStringAsync();
			return resultJson;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return null;
		}
	}
}