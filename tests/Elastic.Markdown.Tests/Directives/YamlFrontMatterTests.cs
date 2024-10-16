using FluentAssertions;

namespace Elastic.Markdown.Tests.Directives;

public class YamlFrontMatterTests() : DirectiveTest(
"""
---
title: Elastic Docs v3
---
"""
)
{
	[Fact]
	public void Test1() => File.Title.Should().Be("Elastic Docs v3");
}
