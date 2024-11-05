// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using System.IO.Abstractions.TestingHelpers;
using Elastic.Markdown.IO;
using Elastic.Markdown.Myst;
using Elastic.Markdown.Myst.Directives;
using FluentAssertions;
using JetBrains.Annotations;
using Markdig.Syntax;
using Microsoft.Extensions.Logging.Abstractions;

namespace Elastic.Markdown.Tests.Directives;

public abstract class DirectiveTest<TDirective>([LanguageInjection("markdown")]string content) : DirectiveTest(content)
	where TDirective : DirectiveBlock
{
	protected TDirective? Block { get; private set; }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		Block = Document
			.Where(block => block is TDirective)
			.Cast<TDirective>()
			.FirstOrDefault();
	}

	[Fact]
	public void BlockIsNotNull() => Block.Should().NotBeNull();

}
public abstract class DirectiveTest : IAsyncLifetime
{
	protected MarkdownFile File { get; }
	protected string Html { get; private set; }
	protected MarkdownDocument Document { get; private set; }
	protected MockFileSystem FileSystem { get; }

	protected DirectiveTest([LanguageInjection("markdown")]string content)
	{
		var logger = NullLoggerFactory.Instance;
		FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
		{
			{ "docs/source/index.md", new MockFileData(content) }
		}, new MockFileSystemOptions
		{
			CurrentDirectory = Paths.Root.FullName
		});

		var file = FileSystem.FileInfo.New("docs/source/index.md");
		var root = FileSystem.DirectoryInfo.New(Paths.Root.FullName);
		var context = new BuildContext { ReadFileSystem = FileSystem, WriteFileSystem = FileSystem };
		var parser = new MarkdownParser(root, context);

		File = new MarkdownFile(file, root, parser, context);
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
