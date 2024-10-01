using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Slugify;

namespace Elastic.Markdown.Myst;

public class SectionedHeadingRenderer : HtmlObjectRenderer<HeadingBlock>
{
	private readonly SlugHelper _slugHelper = new();
	private static readonly string[] HeadingTexts =
	[
		"h1",
		"h2",
		"h3",
		"h4",
		"h5",
		"h6"
	];

	protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
	{
		var index = obj.Level - 1;
		var headings = HeadingTexts;
		var headingText = ((uint)index < (uint)headings.Length)
			? headings[index]
			: $"h{obj.Level}";

		var slug = string.Empty;
		if (headingText == "h2")
		{
			renderer.Write(@"<section id=""");
			slug = _slugHelper.GenerateSlug(obj.Inline?.FirstChild?.ToString());
			renderer.Write(slug);
			renderer.Write(@""">");

		}

		renderer.Write('<');
		renderer.Write(headingText);
		renderer.WriteAttributes(obj);
		renderer.Write('>');

		renderer.WriteLeafInline(obj);


		if (headingText == "h2")
			// language=html
			renderer.WriteLine($@"<a class=""headerlink"" href=""#{slug}"" title=""Link to this heading"">¶</a>");

		renderer.Write("</");
		renderer.Write(headingText);
		renderer.WriteLine('>');

		if (headingText == "h2")
			renderer.Write("</section>");

		renderer.EnsureLine();
	}
}
