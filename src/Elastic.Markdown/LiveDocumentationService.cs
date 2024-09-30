using Elastic.Markdown.Commands;
using Elastic.Markdown.DocSet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elastic.Markdown;


public class LiveDocumentationHolder(string? path, string? output)
{
	private string? Path { get; } = path;
	private string? Output { get; } = output;

	private MystSampleGenerator _generator = new(path, output);
	public MystSampleGenerator Generator => _generator;

	public async Task Reload(CancellationToken ctx)
	{
		var generator = new MystSampleGenerator(Path, Output);
		await generator.ResolveDirectoryTree(ctx);
		Interlocked.Exchange(ref _generator, generator);
	}

	public async Task ReloadNavigation(MarkdownFile current, CancellationToken ctx) =>
		await Generator.ReloadNavigation(current, ctx);
}

public class LiveDocumentationService(LiveDocumentationHolder liveDocumentation, ILogger<LiveDocumentationHolder> logger) : IHostedService
{
	private FileSystemWatcher? _watcher;
	private LiveDocumentationHolder LiveDocumentation { get; } = liveDocumentation;
	public ILogger Logger { get; } = logger;

	//debounce reload requests due to many file changes
	private readonly Debouncer _debouncer = new(TimeSpan.FromMilliseconds(200));

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await LiveDocumentation.Reload(cancellationToken);

		var watcher = new FileSystemWatcher(LiveDocumentation.Generator.DocumentationSet.SourcePath.FullName);

		watcher.NotifyFilter = NotifyFilters.Attributes
		                       | NotifyFilters.CreationTime
		                       | NotifyFilters.DirectoryName
		                       | NotifyFilters.FileName
		                       | NotifyFilters.LastWrite
		                       | NotifyFilters.Security
		                       | NotifyFilters.Size;

		watcher.Changed += OnChanged;
		watcher.Created += OnCreated;
		watcher.Deleted += OnDeleted;
		watcher.Renamed += OnRenamed;
		watcher.Error += OnError;

		watcher.Filter = "*.md";
		watcher.IncludeSubdirectories = true;
		watcher.EnableRaisingEvents = true;
		_watcher = watcher;

		await Task.CompletedTask;
	}

	private void Reload() =>
		_ = _debouncer.ExecuteAsync(async ctx =>
		{
			Logger.LogInformation("Reload due to changes!");
			await LiveDocumentation.Reload(ctx);
			Logger.LogInformation("Reload complete!");
		}, default);

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_watcher?.Dispose();
		return Task.CompletedTask;
	}

	private void OnChanged(object sender, FileSystemEventArgs e)
	{
		if (e.ChangeType != WatcherChangeTypes.Changed)
			return;

		if (e.FullPath.EndsWith("index.md"))
			Reload();

		Logger.LogInformation($"Changed: {e.FullPath}");
	}

	private void OnCreated(object sender, FileSystemEventArgs e)
	{
		var value = $"Created: {e.FullPath}";
		if (e.FullPath.EndsWith(".md"))
			Reload();
		Logger.LogInformation(value);
	}

	private void OnDeleted(object sender, FileSystemEventArgs e)
	{
		if (e.FullPath.EndsWith(".md"))
			Reload();
		Logger.LogInformation($"Deleted: {e.FullPath}");
	}

	private void OnRenamed(object sender, RenamedEventArgs e)
	{
		Logger.LogInformation($"Renamed:");
		Logger.LogInformation($"    Old: {e.OldFullPath}");
		Logger.LogInformation($"    New: {e.FullPath}");
		if (e.FullPath.EndsWith(".md"))
			Reload();

	}

	private void OnError(object sender, ErrorEventArgs e) =>
		PrintException(e.GetException());

	private void PrintException(Exception? ex)
	{
		if (ex == null) return;
		Logger.LogError($"Message: {ex.Message}");
		Logger.LogError("Stacktrace:");
		Logger.LogError(ex.StackTrace);
		PrintException(ex.InnerException);
	}

	private class Debouncer
	{
		private readonly SemaphoreSlim _semaphore = new(1, 1);
		private readonly long _windowInTicks;
		private long _nextRun;

		public Debouncer(TimeSpan window) => _windowInTicks = window.Ticks;

		public async Task ExecuteAsync(Func<CancellationToken, Task> innerAction, CancellationToken cancellationToken)
		{
			var requestStart = DateTime.UtcNow.Ticks;

			try
			{
				await _semaphore.WaitAsync(cancellationToken);

				if (requestStart <= _nextRun)
					return;

				await innerAction(cancellationToken);

				_nextRun = requestStart + _windowInTicks;
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}
