// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Text.Json.Serialization;

namespace Elastic.Markdown.IO;

public record LinkReference
{
	[JsonPropertyName("origin")]
	public required GitConfiguration Origin { get; init; }

	[JsonPropertyName("url_path_prefix")]
	public required string? UrlPathPrefix { get; init; }

	[JsonPropertyName("internal_links")]
	public required string[] InternalLinks { get; init; } = [];

	[JsonPropertyName("external_links")]
	public required string[] ExternalLinks { get; init; } = [];

	public static LinkReference Create(DocumentationSet set)
	{
		var markdownFiles = set.FlatMappedFiles.Values.OfType<MarkdownFile>().ToArray();
		var internalLinks = markdownFiles.Select(m => m.RelativePath).ToArray();
		var externalLinks = markdownFiles.SelectMany(m => m.Links).ToHashSet().ToArray();
		return new LinkReference
		{
			UrlPathPrefix = set.Context.UrlPathPrefix,
			Origin = set.Context.Git,
			InternalLinks = internalLinks,
			ExternalLinks = externalLinks
		};
	}

}
