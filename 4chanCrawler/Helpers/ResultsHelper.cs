using _4chanCrawler.Models;
using Newtonsoft.Json;

namespace _4chanCrawler.Helpers;

public static class ResultsHelper
{
	private const string ResultsFileName = "results.json";
	public static List<Result> Results { get; private set; } = new();

	public static void Add(Result result)
	{
		if (Results.Any(x => x.BoardKey == result.BoardKey && x.ReplyId == result.ReplyId && x.ThreadId == result.ThreadId && x.Keyword == result.Keyword && x.Source == result.Source))
		{
			return;
		}
		Results.Add(result);
		WriteToFile();
	}

	private static void WriteToFile()
	{
		var json = JsonConvert.SerializeObject(Results, Formatting.Indented);
		File.WriteAllText(ResultsFileName, json);
	}

	public static async Task ReadFromFile(bool removeUnavailableResults)
	{
		if (!File.Exists(ResultsFileName))
		{
			return;
		}
		var json = await File.ReadAllTextAsync(ResultsFileName);
		if (string.IsNullOrWhiteSpace(json))
		{
			return;
		}
		Results = JsonConvert.DeserializeObject<List<Result>>(json)!;

		if (removeUnavailableResults)
		{
			Results = Results.Where(x => !x.IsAvailable).ToList();
		}
		await Check();
	}

	private static async Task Check()
	{
		foreach (var result in Results.Where(x => x.IsAvailable))
		{
			try
			{
				var client = new HttpClient();
				var requestResult = await client.GetAsync(result.Url);

				result.IsAvailable = requestResult.IsSuccessStatusCode;
			}
			catch (Exception)
			{
				result.IsAvailable = false;
			}
		}

		WriteToFile();
	}
}