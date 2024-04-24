using System.Web;
using _4chanCrawler.Models;
using _4chanCrawler.Models._4ChanApi;
using _4chanCrawler.Models.Configuration;
using Newtonsoft.Json;

namespace _4chanCrawler.Helpers;

public class CheckHelper
{
	private Guid RunId { get; } = Guid.NewGuid();
	private readonly CrawlerConfiguration _crawlerConfiguration;
	private readonly HttpClientHelper _httpClientHelper;
	private readonly TimeHelper _timeHelper;

	public CheckHelper(CrawlerConfiguration crawlerConfiguration, HttpClientHelper httpClientHelper, TimeHelper timeHelper)
	{
		_crawlerConfiguration = crawlerConfiguration ?? throw new ArgumentNullException(nameof(crawlerConfiguration));
		_httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
		_timeHelper = timeHelper ?? throw new ArgumentNullException(nameof(timeHelper));
	}

	public async Task CheckBoards()
	{
		for (var boardIndex = 0; boardIndex < _crawlerConfiguration.Boards.Count; boardIndex++)
		{
			Console.Write($"Board {boardIndex + 1} / {_crawlerConfiguration.Boards.Count}  ");
			_timeHelper.CalculateTime(_crawlerConfiguration, boardIndex);
			var boardKey = _crawlerConfiguration.Boards[boardIndex].Key;
			var board = await GetBoard(boardKey);
			if (board is null)
			{
				continue;
			}
			await CheckBoardPages(board, _crawlerConfiguration.Boards[boardIndex], _crawlerConfiguration.Keywords, _crawlerConfiguration.TimeoutBetweenRequestsMilliSeconds);
			Thread.Sleep(_crawlerConfiguration.TimeoutBetweenRequestsMilliSeconds);
			var foundCount = ResultsHelper.Results.Count(x => x.RunId == RunId && x.BoardKey == boardKey);
			if (foundCount > 0)
			{
				Console.WriteLine($"\t{foundCount} Found");
			}
			Console.WriteLine();
		}
	}

	private async Task<Board?> GetBoard(string board)
	{
		var url = $"https://a.4cdn.org/{board}/catalog.json";
		var resultJson = await _httpClientHelper.GetJson(url);
		if (string.IsNullOrWhiteSpace(resultJson))
		{
			return null;
		}
		var pages = JsonConvert.DeserializeObject<List<BoardPage>>(resultJson);
		var result = new Board
		{
			BoardName = board,
			Pages = pages
		};
		return result;
	}

	private async Task CheckBoardPages(Board board, BoardConfiguration boardConfiguration, List<string> keywords, int timeoutBetweenRequestsMilliSeconds)
	{
		var pageIndex = 1;
		foreach (var boardPage in board.Pages)
		{
			Console.WriteLine($"\tPage {pageIndex} / {board.Pages.Count}");
			await CheckThreads(boardConfiguration, boardPage.Threads, keywords, timeoutBetweenRequestsMilliSeconds);
			pageIndex++;
		}
	}

	private async Task CheckThreads(BoardConfiguration boardConfiguration, List<BoardThread> threads, List<string> keywords, int timeoutBetweenRequestsMilliSeconds)
	{
		var threadIndex = 1;
		foreach (var thread in threads)
		{
			var threadTitleFoundKey = CheckStringForKeywords(thread.Title, keywords);
			if (threadTitleFoundKey is not null)
			{
				var result = new Result
				{
					RunId = RunId,
					BoardKey = boardConfiguration.Key,
					BoardLabel = boardConfiguration.Label,
					Keyword = threadTitleFoundKey,
					ThreadId = thread.Id,
					Source = "ThreadTitle",
					Url = thread.GetThreadUrl(boardConfiguration.Key)
				};
				ResultsHelper.Add(result);
			}

			var threadTextFoundKey = CheckStringForKeywords(thread.Text, keywords);
			if (threadTextFoundKey is not null)
			{
				var result = new Result
				{
					RunId = RunId,
					BoardKey = boardConfiguration.Key,
					BoardLabel = boardConfiguration.Label,
					Keyword = threadTextFoundKey,
					ThreadId = thread.Id,
					Source = "ThreadText",
					Url = thread.GetThreadUrl(boardConfiguration.Key)
				};
				ResultsHelper.Add(result);
			}
			
			await CheckReplies(boardConfiguration, thread.Id, keywords, timeoutBetweenRequestsMilliSeconds);
			threadIndex++;
		}
	}

	private async Task CheckReplies(BoardConfiguration boardConfiguration, long threadId, List<string> keywords, int timeoutBetweenRequestsMilliSeconds)
	{
		Thread.Sleep(timeoutBetweenRequestsMilliSeconds);
		var resultJson = await _httpClientHelper.GetJson($"https://a.4cdn.org/{boardConfiguration.Key}/thread/{threadId}.json");
		if (string.IsNullOrWhiteSpace(resultJson))
		{
			return;
		}

		var post = JsonConvert.DeserializeObject<BoardThreadPost>(resultJson);

		foreach (var reply in post.Replies)
		{
			var lastReplyTextFoundKey = CheckStringForKeywords(reply.Text, keywords);
			if (lastReplyTextFoundKey is not null)
			{
				var result = new Result
				{
					RunId = RunId,
					BoardKey = boardConfiguration.Key,
					BoardLabel = boardConfiguration.Label,
					Keyword = lastReplyTextFoundKey,
					ThreadId = threadId,
					ReplyId = reply.Id,
					Source = "ReplyText",
					Url = reply.GetReplyUrl(boardConfiguration.Key, threadId)
				};
				ResultsHelper.Add(result);
			}

			if (reply.HasImage)
			{
				var lastReplyFileNameFoundKey = CheckStringForKeywords(reply.FileName, keywords);
				if (lastReplyFileNameFoundKey is not null)
				{
					var result = new Result
					{
						RunId = RunId,
						BoardKey = boardConfiguration.Key,
						BoardLabel = boardConfiguration.Label,
						Keyword = lastReplyFileNameFoundKey,
						ThreadId = threadId,
						ReplyId = reply.Id,
						Source = "ReplyFileName",
						Url = reply.GetReplyUrl(boardConfiguration.Key, reply.Id)
					};
					ResultsHelper.Add(result);
				}
			}
		}
	}

	private string? CheckStringForKeywords(string str, List<string> keywords)
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

	private string AppendKeywordHighlightToUrl(bool doAppend, string url, string keyword)
	{
		if (!doAppend)
		{
			return url;
		}
		var template = "#:~:text=";
		var encodedKeyword = HttpUtility.UrlEncode(keyword);
		var urlWithHighlight = $"{url}{template}{encodedKeyword}";
		return urlWithHighlight;
	}
}