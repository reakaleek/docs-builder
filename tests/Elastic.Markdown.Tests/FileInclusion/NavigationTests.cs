using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Elastic.Markdown.IO;
using Elastic.Markdown.Myst.Directives;
using Elastic.Markdown.Tests.Directives;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Elastic.Markdown.Tests.SiteMap;


public class IncludeTests() : DirectiveTest<IncludeBlock>(
"""
```{include} snippets/test.md
```
"""
)
{
	public override Task InitializeAsync()
	{
		// language=markdown
		var inclusion = "*Hello world*";
		FileSystem.AddFile(@"docs/source/snippets/test.md", inclusion);
		return base.InitializeAsync();
	}

	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void IncludesInclusionHtml() =>
		Html.Should()
			.Contain("Hello world")
			.And.Be("<p><em>Hello world</em></p>\n")
		;
}


public class IncludeSubstitutionTests() : DirectiveTest<IncludeBlock>(
"""
---
title: My Document
sub:
  foo: "bar"
---
```{include} snippets/test.md
```
"""
)
{
	public override Task InitializeAsync()
	{
		// language=markdown
		var inclusion = "*Hello {{foo}}*";
		FileSystem.AddFile(@"docs/source/snippets/test.md", inclusion);
		return base.InitializeAsync();
	}

	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void InclusionInheritsYamlContext() =>
		Html.Should()
			.Contain("Hello bar")
			.And.Be("<p><em>Hello bar</em></p>\n")
		;
}
