using _4chanCrawler.Models.Configuration;
using Newtonsoft.Json;

namespace _4chanCrawler.Helpers;

public static class ConfigurationHelper
{
	private const string ConfigurationFileName = "appsettings.json";
	
	public static CrawlerConfiguration Get()
	{
		var json = File.ReadAllText(ConfigurationFileName);
		var configuration = JsonConvert.DeserializeObject<CrawlerConfiguration>(json);
		
		// Filter Boards
		configuration.Boards = configuration.Boards.Where(x => x.IsEnabled).ToList();
		if (configuration.IgnoreAdultBoards)
		{
			configuration.Boards = configuration.Boards.Where(x => !x.IsAdult).ToList();
		}
		
		// Warnings
		if (configuration.Boards.Count > 15)
		{
			Console.WriteLine($"WARNING: You selected more than 15 Boards ({configuration.Boards.Count}) which will take a lot of time and will send A LOT of requests to 4chan.");
		}

		if (configuration.TimeoutBetweenRequestsMilliSeconds < 500)
		{
			Console.WriteLine($"WARNING: Property '{nameof(configuration.TimeoutBetweenRequestsMilliSeconds)}' is lower than 500ms. It should be 1000ms. Will be set to 500ms.");
			configuration.TimeoutBetweenRequestsMilliSeconds = 500;
		}
		
		return configuration;
	}
}