using FluentAssertions;
using Markdig.Syntax.Inlines;

namespace Elastic.Markdown.Tests.Inline;

public class InlineImageTest() : InlineTest<LinkInline>(
	"""
	![Elasticsearch](/_static/img/observability.png){w=350px align=center}
	"""
)
{
	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void GeneratesAttributesInHtml() =>
		// language=html
		Html.Should().Contain(
			"""<p><img src="/_static/img/observability.png" w="350px" align="center" alt="Elasticsearch" /></p>"""
		);
}
