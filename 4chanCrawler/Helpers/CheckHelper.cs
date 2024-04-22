using _4chanCrawler.Models;
using _4chanCrawler.Models._4ChanApi;
using _4chanCrawler.Models.Configuration;
using Newtonsoft.Json;

namespace _4chanCrawler.Helpers;

public static class CheckHelper
{
	public static async Task<List<Result>> CheckBoards(CrawlerConfiguration configuration)
	{
		// Console.WriteLine("Querying...");
		var results = new List<Result>();
		
		var progressBar = new ProgressBar("Querying...", "Boards", 0, 3);
		for (var boardIndex = 0; boardIndex < configuration.Boards.Count; boardIndex++)
		{
			var board = await GetBoard(configuration.Boards[boardIndex].Key);
			var boardResults = await CheckBoardPages(board, configuration.Boards[boardIndex], configuration.Keywords, configuration.TimeoutBetweenRequestsMilliSeconds);
			results.AddRange(boardResults);
			progressBar.Report((double)boardIndex / (double)configuration.Boards.Count);
			
			Thread.Sleep(configuration.TimeoutBetweenRequestsMilliSeconds);
		}

		return results;
	}

	private static async Task<Board> GetBoard(string board)
	{
		var url = $"https://a.4cdn.org/{board}/catalog.json";
		var client = new HttpClient();
		var clientResult = await client.GetAsync(url);
		if (!clientResult.IsSuccessStatusCode)
		{
			throw new Exception($"Can't get board data for {board}");
		}

		var resultData = await clientResult.Content.ReadAsStringAsync();
		var pages = JsonConvert.DeserializeObject<List<BoardPage>>(resultData);
		var result = new Board
		{
			BoardName = board,
			Pages = pages
		};
		return result;
	}

	private static async Task<List<Result>> CheckBoardPages(Board board, BoardConfiguration boardConfiguration, List<string> keywords, int timeoutBetweenRequestsMilliSeconds)
	{
		var results = new List<Result>();
		var pageIndex = 1;
		var progressBar = new ProgressBar("Querying...", "Pages", 2, 3);
		foreach (var boardPage in board.Pages)
		{
			var threadResults = CheckThreads(boardConfiguration, boardPage.Threads, keywords, timeoutBetweenRequestsMilliSeconds);
			results.AddRange(await threadResults);
			pageIndex++;
			progressBar.Report((double)pageIndex / (double)board.Pages.Count);
		}
		progressBar.Dispose();
		
		return results;
	}

	private static async Task<List<Result>> CheckThreads(BoardConfiguration boardConfiguration, List<BoardThread> threads, List<string> keywords, int timeoutBetweenRequestsMilliSeconds)
	{
		var results = new List<Result>();
		var threadIndex = 1;
		var progressBar = new ProgressBar("Querying...", "Threads", 2, 3);
		foreach (var thread in threads)
		{
			var threadTitleFoundKey = CheckStringForKeywords(thread.Title, keywords);
			if (threadTitleFoundKey is not null)
			{
				var result = new Result
				{
					BoardKey = boardConfiguration.Key,
					BoardLabel = boardConfiguration.Label,
					Keyword = threadTitleFoundKey,
					ThreadId = thread.Id,
					Source = "ThreadTitle",
					Url = thread.GetThreadUrl(boardConfiguration.Key)
				};
				results.Add(result);
			}

			var threadTextFoundKey = CheckStringForKeywords(thread.Text, keywords);
			if (threadTextFoundKey is not null)
			{
				var result = new Result
				{
					BoardKey = boardConfiguration.Key,
					BoardLabel = boardConfiguration.Label,
					Keyword = threadTextFoundKey,
					ThreadId = thread.Id,
					Source = "ThreadText",
					Url = thread.GetThreadUrl(boardConfiguration.Key)
				};
				results.Add(result);
			}
			
			var repliesResults = await CheckReplies(boardConfiguration, thread.Id, keywords, timeoutBetweenRequestsMilliSeconds);
			results.AddRange(repliesResults);
			progressBar.Report((double)threadIndex / (double)threads.Count);
			threadIndex++;
		}
		progressBar.Dispose();
		return results;
	}

	private static async Task<List<Result>> CheckReplies(BoardConfiguration boardConfiguration, long threadId, List<string> keywords, int timeoutBetweenRequestsMilliSeconds)
	{
		Thread.Sleep(timeoutBetweenRequestsMilliSeconds);
		var results = new List<Result>();
		var client = new HttpClient();
		var httpClientResult = await client.GetAsync($"https://a.4cdn.org/{boardConfiguration.Key}/thread/{threadId}.json");
		
		if (!httpClientResult.IsSuccessStatusCode)
		{
			throw new Exception($"Can't get replies for {boardConfiguration.Key}/{threadId} {httpClientResult.StatusCode} {httpClientResult.ReasonPhrase}");
		}

		var post = JsonConvert.DeserializeObject<BoardThreadPost>(await httpClientResult.Content.ReadAsStringAsync());

		foreach (var reply in post.Replies)
		{
			var lastReplyTextFoundKey = CheckStringForKeywords(reply.Text, keywords);
			if (lastReplyTextFoundKey is not null)
			{
				var result = new Result
				{
					BoardKey = boardConfiguration.Key,
					BoardLabel = boardConfiguration.Label,
					Keyword = lastReplyTextFoundKey,
					ThreadId = threadId,
					ReplyId = reply.Id,
					Source = "ReplyText",
					Url = reply.GetReplyUrl(boardConfiguration.Key, threadId)
				};
				results.Add(result);
			}

			if (reply.HasImage)
			{
				var lastReplyFileNameFoundKey = CheckStringForKeywords(reply.FileName, keywords);
				if (lastReplyFileNameFoundKey is not null)
				{
					var result = new Result
					{
						BoardKey = boardConfiguration.Key,
						BoardLabel = boardConfiguration.Label,
						Keyword = lastReplyFileNameFoundKey,
						ThreadId = threadId,
						ReplyId = reply.Id,
						Source = "ReplyFileName",
						Url = reply.GetReplyUrl(boardConfiguration.Key, reply.Id)
					};
					results.Add(result);
				}
			}
		}

		return results;
	}

	private static string? CheckStringForKeywords(string str, List<string> keywords)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return null;
		}

		foreach (var keyword in keywords)
		{
			if (str.Contains(keyword, StringComparison.OrdinalIgnoreCase))
			{
				return keyword;
			}
		}

		return null;
	}
}