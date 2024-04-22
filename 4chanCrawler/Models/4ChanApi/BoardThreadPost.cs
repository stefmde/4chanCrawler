using Newtonsoft.Json;

namespace _4chanCrawler.Models._4ChanApi;

// https://a.4cdn.org/po/thread/570368.json
// https://github.com/4chan/4chan-API/blob/master/pages/Threads.md
public class BoardThreadPost
{
	[JsonProperty(PropertyName = "posts")]
	public List<BoardThreadReply> Replies { get; set; }
}