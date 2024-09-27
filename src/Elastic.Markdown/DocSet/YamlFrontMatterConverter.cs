using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Elastic.Markdown.DocSet;

[YamlStaticContext]
public partial class YamlFrontMatterStaticContext;

[YamlSerializable]
public class YamlFrontMatter
{
	public string? Title { get; set; }
}

public class YamlFrontMatterConverter
{
	public YamlFrontMatter Deserialize(string yaml)
	{
		var input = new StringReader(yaml);

		var deserializer = new StaticDeserializerBuilder(new YamlFrontMatterStaticContext())
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var frontMatter = deserializer.Deserialize<YamlFrontMatter>(input);
		return frontMatter;

	}
}

