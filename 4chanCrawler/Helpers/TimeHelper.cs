using _4chanCrawler.Models.Configuration;

namespace _4chanCrawler.Helpers;

public class TimeHelper
{
	public static TimeSpan CalculateTime(CrawlerConfiguration configuration, int currentBoardIndex = -1)
	{
		var seconds = ((double)configuration.Boards.Count - (double)currentBoardIndex + 1) * 10 * (double)((double)configuration.TimeoutBetweenRequestsMilliSeconds / (double)1000);
		var ts = TimeSpan.FromSeconds(seconds);

		if (currentBoardIndex == -1)
		{
			Console.WriteLine($"ETA (HH:MM:SS): {ts} which will be at {DateTime.Now.Add(ts)}");
		}
		
		return ts;
	}
}