using Newtonsoft.Json;

namespace _4chanCrawler.Models._4ChanApi;

public class BoardThreadBaseData
{
	[JsonProperty(PropertyName = "no")]
	public long Id { get; set; }
	
	[JsonProperty(PropertyName = "com")]
	public string? Text { get; set; }
	
	[JsonProperty(PropertyName = "filename")]
	public string? FileName { get; set; }
	
	[JsonProperty(PropertyName = "tim")]
	public long FileId { get; set; }
	
	[JsonProperty(PropertyName = "ext")]
	public string? FileExtension { get; set; }
	
	[JsonProperty(PropertyName = "w")]
	public int ImageWidth { get; set; }
	
	[JsonProperty(PropertyName = "h")]
	public int ImageHeight { get; set; }
	
	[JsonProperty(PropertyName = "time")]
	public int TimeStamp { get; set; }
	
	[JsonProperty(PropertyName = "fsize")]
	public int FileSize { get; set; }

	public bool HasImage => !string.IsNullOrWhiteSpace(FileName) && FileId > 0 && ImageWidth > 0 && FileSize > 0;

	public string GetImageUrl(string board)
	{
		if (!HasImage)
		{
			return null;
		}

		return $"https://i.4cdn.org/{board}/{FileId}{FileExtension}";
	}
}