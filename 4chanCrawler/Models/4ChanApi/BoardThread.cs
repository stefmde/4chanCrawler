using Newtonsoft.Json;

namespace _4chanCrawler.Models._4ChanApi;

// https://a.4cdn.org/hc/catalog.json
// https://github.com/4chan/4chan-API/blob/master/pages/Catalog.md
public class BoardThread : BoardThreadBaseData
{
	[JsonProperty(PropertyName = "sub")]
	public required string Title { get; set; }
	
	[JsonProperty(PropertyName = "replies")]
	public int RepliesCount { get; set; }
	
	[JsonProperty(PropertyName = "images")]
	public int ImagesCount { get; set; }
	
	// [JsonProperty(PropertyName = "last_replies")]
	// public List<ThreadsLastReply> LastReplies { get; set; }
	
	public string GetThreadUrl(string board)
	{
		if (!HasImage)
		{
			return null;
		}

		return $"https://boards.4chan.org/{board}/thread/{Id}/";
	}
}