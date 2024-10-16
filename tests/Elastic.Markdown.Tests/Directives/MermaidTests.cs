using Elastic.Markdown.Myst.Directives;
using FluentAssertions;
using Xunit;

namespace Elastic.Markdown.Tests.Directives;

public class MermaidBlockTests() : DirectiveTest<MermaidBlock>(
"""
```{mermaid} /_static/img/observability.png
flowchart LR
  A[Jupyter Notebook] --> C
  B[MyST Markdown] --> C
  C(mystmd) --> D{AST}
  D <--> E[LaTeX]
  E --> F[PDF]
  D --> G[Word]
  D --> H[React]
  D --> I[HTML]
  D <--> J[JATS]
```
"""
)
{
	[Fact]
	public void ParsesBlock() => Block.Should().NotBeNull();

	[Fact]
	public void IncludesRawFlowChart() =>
		Html.Should().Contain("D --&gt; I[HTML]");
}
