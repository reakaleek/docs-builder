using Cysharp.IO;
using Markdig;
using Markdig.Syntax;

namespace Elastic.Markdown;

public class Template(string name, string contents)
{
	public string Name { get; } = name;
	public string Contents { get; } = contents;
};

public class DocSetConverter(string? sourcePath, string? outputPath)
{
	public string SourcePath { get; } = sourcePath ?? ".artifacts/docset-source";
	public string OutputPath { get; } = outputPath ?? ".artifacts/docset-generated";

	public static MarkdownPipeline DefaultPipeline =
		new MarkdownPipelineBuilder()
            .UseAlertBlocks()
            .UseAbbreviations()
            .UseAutoIdentifiers()
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseMathematics()
            .UseMediaLinks()
            .UsePipeTables()
            .UseListExtras()
            .UseTaskLists()
            .UseAutoLinks()
            //.UseGridTables()
            //.UseDiagrams()
            .UseGenericAttributes() // Must be last as it is one parser that is modifying other parsers
			//.EnableTrackTrivia()
			.DisableHtml()
			.Build();

	public async Task Build()
	{
		var dir = new DirectoryInfo(OutputPath);
		if (OutputPath.StartsWith(".artifacts") && dir.Exists)
			dir.Delete(true);
		dir.Create();

		var files = Directory.EnumerateFiles(SourcePath, "*.*", SearchOption.AllDirectories);

		/*
		foreach (var file in files)
		{
			await Task.CompletedTask;
		}

		*/
		/*
		foreach (var file in files)
		{
			var parsed = await ParseAsync(file);
			var relative = Path.GetRelativePath(SourcePath, file);
			var outputPath = new FileInfo(Path.Combine(OutputPath, relative.Replace(".md", ".html")));
			Directory.CreateDirectory(outputPath.Directory!.FullName);
			var html = parsed.ToHtml(DefaultPipeline);
			await File.WriteAllTextAsync(outputPath.FullName, html);
		}
		*/

		await Parallel.ForEachAsync(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (file, ctx) =>
		{
			var parsed = await ParseAsync(file);
			var relative = Path.GetRelativePath(SourcePath, file);
			var outputPath = new FileInfo(Path.Combine(OutputPath, relative.Replace(".md", ".html")));
			Directory.CreateDirectory(outputPath.Directory!.FullName);
			var html = parsed.ToHtml(DefaultPipeline);
			await File.WriteAllTextAsync(outputPath.FullName, html);
		});
		await Task.CompletedTask;
	}

	public static async Task<MarkdownDocument> ParseAsync(string path)
	{
		await using var streamReader = new Utf8StreamReader(path, fileOpenMode: FileOpenMode.Throughput);
		var inputMarkdown = await streamReader.AsTextReader().ReadToEndAsync();
		var context = new MarkdownParserContext();
		var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, DefaultPipeline, context);
		return markdownDocument;
	}
}

public static class TaskExtensions
{
	// Temporarily until dotnet 9 is released, this is not ordered by completion
	public static async IAsyncEnumerable<T> WhenEach<T>(this IEnumerable<Task<T>> tasks)
	{
		foreach (var task in tasks)
			yield return await task;
	}
}
