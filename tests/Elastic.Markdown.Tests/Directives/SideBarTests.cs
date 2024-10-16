using Elastic.Markdown.Myst.Directives;
using FluentAssertions;

namespace Elastic.Markdown.Tests.Directives;

public class SideBarTests() : DirectiveTest<SideBarBlock>(
"""
```{sidebar}
This code is very helpful.

It does lots of things.

But it does not sing.
```
"""
)
{
	[Fact]
	public void ParsesBlock () => Block.Should().NotBeNull();

}
