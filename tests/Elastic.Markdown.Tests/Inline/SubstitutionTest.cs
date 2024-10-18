using Elastic.Markdown.Myst.Substitution;
using FluentAssertions;

namespace Elastic.Markdown.Tests.Inline;

public class SubstitutionTest() : LeafTest<SubstitutionLeaf>(
"""
---
sub:
  hello-world: "Hello World!"
---
The following should be subbed: {{hello-world}}
not a comment
"""
)
{

	[Fact]
	public void GeneratesAttributesInHtml() =>
		Html.Should().Contain(
				"""Hello World!<br />"""
			).And.Contain(
				"""not a comment"""
			)
			.And.NotContain(
				"""{{hello-world}}"""
			);
}

public class NeedsDoubleBrackets() : InlineTest(
"""
---
sub:
  hello-world: "Hello World!"
---
The following should be subbed: {{hello-world}}
not a comment
not a {{valid-key}}
not a {substitution}
"""
)
{

	[Fact]
	public void GeneratesAttributesInHtml() =>
		Html.Should().Contain(
				"""Hello World!<br />"""
			).And.Contain(
				"""not a comment"""
			)
			.And.NotContain(
				"""{{hello-world}}"""
			)
			.And.NotContain( // treated as attributes to the block
				"""{substitution}"""
			)
			.And.Contain(
				"""{{valid-key}}"""
			);
}
