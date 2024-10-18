using System.IO.Abstractions.TestingHelpers;
using Elastic.Markdown.IO;
using Elastic.Markdown.Myst;
using Elastic.Markdown.Myst.Directives;
using FluentAssertions;
using JetBrains.Annotations;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Logging.Abstractions;

namespace Elastic.Markdown.Tests.Inline;

public abstract class LeafTest<TDirective>([LanguageInjection("markdown")]string content) : InlineTest(content)
	where TDirective : LeafInline
{
	protected TDirective? Block { get; private set; }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		Block = Document
			.Where(block => block is ParagraphBlock)
			.Cast<ParagraphBlock>()
			.FirstOrDefault()?
			.Inline?
			.Where(block => block is TDirective)
			.Cast<TDirective>()
			.FirstOrDefault();
	}

	[Fact]
	public void BlockIsNotNull() => Block.Should().NotBeNull();

}

public abstract class InlineTest<TDirective>([LanguageInjection("markdown")]string content) : InlineTest(content)
	where TDirective : ContainerInline
{
	protected TDirective? Block { get; private set; }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		Block = Document
			.Where(block => block is ParagraphBlock)
			.Cast<ParagraphBlock>()
			.FirstOrDefault()?
			.Inline?
			.Where(block => block is TDirective)
			.Cast<TDirective>()
			.FirstOrDefault();
	}

	[Fact]
	public void BlockIsNotNull() => Block.Should().NotBeNull();

}
public abstract class InlineTest : IAsyncLifetime
{
	protected MarkdownFile File { get; }
	protected string Html { get; private set; }
	protected MarkdownDocument Document { get; private set; }

	protected InlineTest([LanguageInjection("markdown")]string content)
	{
		var logger = NullLoggerFactory.Instance;
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
		{
			{ "docs/source/index.md", new MockFileData(content) }
		}, new MockFileSystemOptions
		{
			CurrentDirectory = Paths.Root.FullName
		});

		var file = fileSystem.FileInfo.New("docs/source/index.md");
		var root = fileSystem.DirectoryInfo.New(Paths.Root.FullName);
		var parser = new MarkdownParser();

		File = new MarkdownFile(file, root, parser);
		Html = default!; //assigned later
		Document = default!;
	}

	public virtual async Task InitializeAsync()
	{
		Document = await File.ParseFullAsync(default);
		Html = await File.CreateHtmlAsync(File.YamlFrontMatter, default);
	}

	public Task DisposeAsync() => Task.CompletedTask;

}
