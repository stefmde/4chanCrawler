namespace _4chanCrawler.Models._4ChanApi;

// https://github.com/4chan/4chan-API
public class Board
{
	public required string BoardName { get; set; }
	
	public List<BoardPage> Pages { get; set; }
}