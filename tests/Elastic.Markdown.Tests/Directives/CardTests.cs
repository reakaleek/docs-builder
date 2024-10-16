using Elastic.Markdown.Myst.Directives;
using FluentAssertions;

namespace Elastic.Markdown.Tests.Directives;

public class CardTests() : DirectiveTest<CardBlock>(
"""
```{card} Card title
Card content
```
"""
)
{
	[Fact]
	public void ParsesBlock () => Block.Should().NotBeNull();

}

public class LinkCardTests() : DirectiveTest<CardBlock>(
"""
```{card} Clickable Card
:link: https://elastic.co/docs

The entire card can be clicked to navigate to `Elastic Docs`.
```
"""
)
{
	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void ExposesLink() => Block!.Link.Should().Be("https://elastic.co/docs");

}
