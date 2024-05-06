
namespace _4chanCrawler.Helpers;

public class HttpClientHelper
{
	private readonly HttpClient _client = new ();
	public int WebRequests { get; private set; }
	
	public async Task<string?> GetJson(string url)
	{
		try
		{
			var resultJson = await _client.GetStringAsync(url);
			WebRequests++;
			return resultJson;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return null;
		}
	}
}