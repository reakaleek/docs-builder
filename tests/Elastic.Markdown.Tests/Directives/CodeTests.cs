// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using Elastic.Markdown.Myst.Directives;
using FluentAssertions;

namespace Elastic.Markdown.Tests.Directives;

public abstract class CodeBlockTests(string directive, string? language = null) : DirectiveTest<CodeBlock>(
$$"""
```{{directive}} {{language}}
var x = 1;
```
A regular paragraph.
"""
)
{
	[Fact]
	public void ParsesAdmonitionBlock() => Block.Should().NotBeNull();

	[Fact]
	public void SetsCorrectDirectiveType() => Block!.Directive.Should().Be(language != null ? directive.Trim('{','}') : "raw");
}

public class CodeBlockDirectiveTests() : CodeBlockTests("{code-block}", "csharp")
{
	[Fact]
	public void SetsLanguage() => Block!.Language.Should().Be("csharp");
}

public class CodeTests() : CodeBlockTests("{code}", "python")
{
	[Fact]
	public void SetsLanguage() => Block!.Language.Should().Be("python");
}

public class SourceCodeTests() : CodeBlockTests("{sourcecode}", "java")
{
	[Fact]
	public void SetsLanguage() => Block!.Language.Should().Be("java");
}

public class RawMarkdownCodeBlockTests() : CodeBlockTests("javascript")
{
	[Fact]
	public void SetsLanguage() => Block!.Language.Should().Be("javascript");
}

