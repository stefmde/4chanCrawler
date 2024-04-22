using System.Text;

namespace _4chanCrawler.Helpers;

/// <summary>
/// An ASCII progress bar
/// Source: https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
/// </summary>
public class ProgressBar : IDisposable, IProgress<double>
{
	private const int BlockCount = 20;
	private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
	private const string Animation = @"|/-\";

	private readonly Timer _timer;

	private double _currentProgress;
	private string _currentText = string.Empty;
	private bool _disposed;
	private int _animationIndex;
	private readonly string? _instanceLabel;
	private readonly string? _title;
	private readonly int _instanceIndex;
	private readonly int _instanceCount;

	public ProgressBar(string? title = null, string? instanceLabel = null, int instanceIndex = 0, int instanceCount = 1)
	{
		_instanceLabel = instanceLabel;
		_title = title;
		_instanceIndex = instanceIndex;
		_instanceCount = instanceCount;
		_timer = new Timer(TimerHandler!);

		// A progress bar is only for temporary display in a console window.
		// If the console output is redirected to a file, draw nothing.
		// Otherwise, we'll end up with a lot of garbage in the target file.
		if (!Console.IsOutputRedirected)
		{
			ResetTimer();
		}
	}

	/// <summary>
	/// Report the progress
	/// </summary>
	/// <param name="value">Between 0..1</param>
	public void Report(double value)
	{
		// Make sure value is in [0..1] range
		value = Math.Max(0, Math.Min(1, value));
		Interlocked.Exchange(ref _currentProgress, value);
	}

	private void TimerHandler(object state)
	{
		lock (_timer)
		{
			if (_disposed)
			{
				return;
			}

			var progressBlockCount = (int) (_currentProgress * BlockCount);
			var percent = (int) (_currentProgress * 100);
			var separator = _instanceCount > 1 ? "    " : "";
			var title = !string.IsNullOrWhiteSpace(_title) && _instanceIndex == 0 ? $"{_title}    " : "";
			var text = string.Format("{0}{1} [{2}{3}] {4,3}% {5}{6}",
				title,
				_instanceLabel,
				new string( '#', progressBlockCount), new string('-', BlockCount - progressBlockCount),
				percent,
				Animation[_animationIndex++ % Animation.Length],
				separator);
			UpdateText(text);

			ResetTimer();
		}
	}

	private void UpdateText(string text)
	{
		// Get length of common portion
		var commonPrefixLength = 0;
		var commonLength = Math.Min(_currentText.Length, text.Length);
		while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
		{
			commonPrefixLength++;
		}

		// Backtrack to the first differing character
		var outputBuilder = new StringBuilder();
		outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

		// Output new suffix
		outputBuilder.Append(text.Substring(commonPrefixLength));

		// If the new text is shorter than the old one: delete overlapping characters
		var overlapCount = _currentText.Length - text.Length;
		if (overlapCount > 0)
		{
			outputBuilder.Append(' ', overlapCount);
			outputBuilder.Append('\b', overlapCount);
		}

		Console.Write(outputBuilder);
		_currentText = text;
	}

	private void ResetTimer()
	{
		_timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
	}

	public void Dispose()
	{
		lock (_timer)
		{
			_disposed = true;
			UpdateText(string.Empty);
		}

		_timer.Dispose();
	}
}