// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using System.IO.Abstractions;

namespace Elastic.Markdown;

public record BuildContext
{
	private readonly string? _urlPathPrefix;
	public bool Force { get; init; }

	public string? UrlPathPrefix
	{
		get => string.IsNullOrWhiteSpace(_urlPathPrefix) ? "" : $"/{_urlPathPrefix.Trim('/')}";
		init => _urlPathPrefix = value;
	}

	public required IFileSystem ReadFileSystem { get; init; }
	public required IFileSystem WriteFileSystem { get; init; }
}
