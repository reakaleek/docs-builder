using FluentAssertions;

namespace Elastic.Markdown.Tests.Inline;

public class CommentTest() : InlineTest(
	"""
	% comment
	not a comment
	"""
)
{

	[Fact]
	public void GeneratesAttributesInHtml() =>
		// language=html
		Html.Should().NotContain(
			"""<p>% comment"""
		)
		.And.Contain(
			"""<p>not a comment</p>"""
		).And.Be(
			"""
			<p>not a comment</p>

			"""
		);
}
