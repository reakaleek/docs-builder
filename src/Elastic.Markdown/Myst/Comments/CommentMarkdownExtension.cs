using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;

namespace Elastic.Markdown.Myst.Comments;

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
			pipeline.BlockParsers.InsertBefore<ThematicBreakParser>(new CommentBlockParser());
	}

	public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
	{
		if (!renderer.ObjectRenderers.Contains<CommentRenderer>())
			renderer.ObjectRenderers.InsertBefore<SectionedHeadingRenderer>(new CommentRenderer());
	}
}
