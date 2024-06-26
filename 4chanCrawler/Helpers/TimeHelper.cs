using _4chanCrawler.Models.Configuration;

namespace _4chanCrawler.Helpers;

public class TimeHelper
{
	private const int PagesPerBoard = 11;
	private const int ThreadsPerBoard = 15;
	private const int ProcessingMs = 350;
	
	public TimeSpan CalculateTime(CrawlerConfiguration configuration, int currentBoardIndex = -1)
	{
		var seconds = ((double)configuration.Boards.Count - (double)currentBoardIndex) * PagesPerBoard * ThreadsPerBoard * (double)((double)(configuration.TimeoutBetweenRequestsMilliSeconds + ProcessingMs) / (double)1000);
		var ts = TimeSpan.FromSeconds(seconds);
		Console.WriteLine($"ETA (HH:MM:SS): {ts.ToString(Constants.TimeSpanFormat)} which will be at {DateTime.Now.Add(ts).ToString(Constants.DateTimeFormat)}");
		
		return ts;
	}
}