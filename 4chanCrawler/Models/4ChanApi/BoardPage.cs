namespace _4chanCrawler.Models._4ChanApi;

// https://a.4cdn.org/hc/catalog.json
// https://github.com/4chan/4chan-API/blob/master/pages/Catalog.md
public class BoardPage
{
	public int Page { get; set; }
	
	public List<BoardThread> Threads { get; set; }
}