using Elastic.Markdown.Myst.Directives;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;

namespace Elastic.Markdown.Myst;

public static class CommentBuilderExtensions
{
	public static MarkdownPipelineBuilder UseComments(this MarkdownPipelineBuilder pipeline)
	{
		pipeline.Extensions.AddIfNotAlready<CommentMarkdownExtension>();
		return pipeline;
	}
}

public class CommentMarkdownExtension : IMarkdownExtension
{
	public void Setup(MarkdownPipelineBuilder pipeline)
	{
		if (!pipeline.BlockParsers.Contains<CommentBlockParser>())
		{
			// Insert the parser before any other parsers
			pipeline.BlockParsers.InsertBefore<ThematicBreakParser>(new CommentBlockParser());
		}
		pipeline.BlockParsers.Replace<ParagraphBlockParser>(new DirectiveParagraphParser());
	}

	public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
	{
		if (!renderer.ObjectRenderers.Contains<CommentRenderer>())
			renderer.ObjectRenderers.InsertBefore<SectionedHeadingRenderer>(new CommentRenderer());

		renderer.ObjectRenderers.Replace<SectionedHeadingRenderer>(new CommentRenderer());
	}
}
