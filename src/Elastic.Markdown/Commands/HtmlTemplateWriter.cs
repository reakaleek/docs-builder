using Elastic.Markdown.Templating;
using Markdig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorSlices;

namespace Elastic.Markdown.Commands;

public class HtmlTemplateWriter
{
	public HtmlTemplateWriter()
	{
		var services = new ServiceCollection();
		services.AddLogging();

		ServiceProvider = services.BuildServiceProvider();
		LoggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
	}

	public ILoggerFactory LoggerFactory { get; }
	public ServiceProvider ServiceProvider { get; }

	public async Task<string> RenderLayout(string markdownHtml, CancellationToken ctx = default)
	{
		var slice = Slices.Index.Create(new MyModel { MarkdownHtml = markdownHtml });
		return await slice.RenderAsync(cancellationToken: ctx);
	}

	public async Task WriteAsync(FileInfo outputFile, string markdownHtml, CancellationToken ctx = default)
	{
		if (outputFile.Directory is { Exists: false })
			outputFile.Directory.Create();
		var rendered = await RenderLayout(markdownHtml, ctx);
		await File.WriteAllTextAsync(outputFile.FullName, rendered, ctx);
	}
}
