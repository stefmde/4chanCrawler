namespace _4chanCrawler.Models.Configuration;

public class BoardConfiguration
{
	public required string Key { get; set; }
	
	public required string Label { get; set; }
	
	public required string Category { get; set; }
	
	public bool IsEnabled { get; set; }
	
	public bool IsAdult { get; set; }
}