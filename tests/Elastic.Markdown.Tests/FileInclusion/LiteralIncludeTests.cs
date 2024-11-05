using Elastic.Markdown.Myst.Directives;
using Elastic.Markdown.Tests.Directives;
using FluentAssertions;

namespace Elastic.Markdown.Tests.FileInclusion;


public class LiteralIncludeUsingPropertyTests() : DirectiveTest<IncludeBlock>(
"""
```{include} snippets/test.txt
:literal: true
```
"""
)
{
	public override Task InitializeAsync()
	{
		// language=markdown
		var inclusion = "*Hello world*";
		FileSystem.AddFile(@"docs/source/snippets/test.txt", inclusion);
		return base.InitializeAsync();
	}

	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void IncludesInclusionHtml() =>
		Html.Should()
			.Be("*Hello world*")
		;
}


public class LiteralIncludeTests() : DirectiveTest<IncludeBlock>(
"""
```{literalinclude} snippets/test.md
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
			.Be("*Hello world*");
}
