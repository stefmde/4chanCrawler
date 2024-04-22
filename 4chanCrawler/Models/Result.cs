namespace _4chanCrawler.Models;

public class Result
{
	public required Guid RunId { get; set; }
	
	public required string BoardKey { get; set; }
	
	public required string BoardLabel { get; set; }
	
	public required string Keyword { get; set; }
	
	public required string Source { get; set; }
	
	public required string Url { get; set; }
	
	public DateTime Created { get; set; } = DateTime.Now;
	
	public bool Available { get; set; } = true;
	
	public long ThreadId { get; set; }
	
	public long? ReplyId { get; set; }

	public override string ToString()
	{
		return $"'{Keyword}' on Board '{BoardLabel}' ({BoardKey}) with Source '{Source}' -> {Url}";
	}
}