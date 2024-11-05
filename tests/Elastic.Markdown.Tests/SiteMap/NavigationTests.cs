// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Elastic.Markdown.IO;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Elastic.Markdown.Tests.SiteMap;

public class NavigationTests
{
	[Fact]
	public async Task CreatesDefaultOutputDirectory()
	{
		var logger = NullLoggerFactory.Instance;
		var readFs = new FileSystem(); //use real IO to read docs.
		var writeFs = new MockFileSystem(new MockFileSystemOptions //use in memory mock fs to test generation
		{
			CurrentDirectory = Paths.Root.FullName
		});

		var set = new DocumentationSet(readFs);

		set.Files.Should().HaveCountGreaterThan(10);
		var context = new BuildContext
		{
			Force = false, UrlPathPrefix = null, ReadFileSystem = readFs, WriteFileSystem = writeFs
		};
		var generator = new DocumentationGenerator(set, context, logger);

		await generator.GenerateAll(default);

		writeFs.Directory.Exists(".artifacts/docs/html").Should().BeTrue();
		readFs.Directory.Exists(".artifacts/docs/html").Should().BeFalse();

	}
}
