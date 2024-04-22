namespace _4chanCrawler.Models._4ChanApi;

// https://a.4cdn.org/s/thread/21847595.json
// https://github.com/4chan/4chan-API/blob/master/pages/Threads.md
public class BoardThreadReply : BoardThreadBaseData
{
	public string GetReplyUrl(string boardKey, long threadId)
	{
		return $"https://boards.4chan.org/{boardKey}/thread/{threadId}/#p{Id}";
	}
}