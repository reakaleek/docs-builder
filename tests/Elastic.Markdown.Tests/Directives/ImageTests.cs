using Elastic.Markdown.Myst.Directives;
using FluentAssertions;
using Xunit;

namespace Elastic.Markdown.Tests.Directives;

public class ImageBlockTests() : DirectiveTest<ImageBlock>(
"""
```{image} /_static/img/observability.png
:alt: Elasticsearch
:width: 250px
```
"""
)
{
	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void ParsesBreakPoint()
	{
		Block!.Alt.Should().Be("Elasticsearch");
		Block!.Width.Should().Be("250px");
		Block!.ImageUrl.Should().Be("/_static/img/observability.png");
	}
}

public class FigureTests() : DirectiveTest<ImageBlock>(
"""
```{figure} https://github.com/rowanc1/pics/blob/main/sunset.png?raw=true
:label: myFigure
:alt: Sunset at the beach
:align: center

Relaxing at the beach 🏝 🌊 😎
```
"""
)
{
	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void ParsesBreakPoint()
	{
	}
}
