using Elastic.Markdown.DocSet;
using Elastic.Markdown.Templating;
using Markdig;
using Markdig.Syntax;
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

	public async Task<string> RenderLayout(DocumentationSet documentationSet, MarkdownFile markdown, CancellationToken ctx = default)
	{
		var html = markdown.CreateHtml();

		await documentationSet.Tree.Resolve(markdown, ctx);
		var slice = Slices.Index.Create(new MyModel
		{
			Title = markdown.Title ?? "[TITLE NOT SET]",
			MarkdownHtml = html,
			PageTocItems = markdown.TableOfContents,
			Tree = documentationSet.Tree
		});
		return await slice.RenderAsync(cancellationToken: ctx);
	}

	public async Task WriteAsync(DocumentationSet documentationSet, FileInfo outputFile, MarkdownFile markdown,
		CancellationToken ctx = default)
	{
		if (outputFile.Directory is { Exists: false })
			outputFile.Directory.Create();
		var rendered = await RenderLayout(documentationSet, markdown, ctx);
		await File.WriteAllTextAsync(outputFile.FullName, rendered, ctx);
	}
}
