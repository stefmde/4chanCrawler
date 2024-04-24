namespace _4chanCrawler.Models.Configuration;

public class CrawlerConfiguration
{
	public int TimeoutBetweenLoopsMinutes { get; set; }
	
	public int TimeoutBetweenRequestsMilliSeconds { get; set; }
	
	public bool IgnoreAdultBoards { get; set; }
	
	public bool RemoveUnavailableResults { get; set; }
	
	public List<string> Keywords { get; set; } = new();
	
	public List<BoardConfiguration> Boards { get; set; } = new();
}