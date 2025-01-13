// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using System.IO.Abstractions;
using Actions.Core.Services;
using ConsoleAppFramework;
using Documentation.Builder.Diagnostics.Console;
using Documentation.Builder.Http;
using Elastic.Markdown;
using Elastic.Markdown.IO;
using Microsoft.Extensions.Logging;
using Documentation.Builder.LinkIndex;

namespace Documentation.Builder.Cli;

internal class Commands(ILoggerFactory logger, ICoreService githubActionsService, ILinkIndex linkIndex)
{
	/// <summary>
	///	Continuously serve a documentation folder at http://localhost:5000.
	/// File systems changes will be reflected without having to restart the server.
	/// </summary>
	/// <param name="path">-p, Path to serve the documentation.
	/// Defaults to the`{pwd}/docs` folder
	/// </param>
	/// <param name="ctx"></param>
	[Command("serve")]
	public async Task Serve(string? path = null, Cancel ctx = default)
	{
		var host = new DocumentationWebHost(path, logger, new FileSystem());
		await host.RunAsync(ctx);
		await host.StopAsync(ctx);
	}

	/// <summary>
	/// Converts a source markdown folder or file to an output folder
	/// </summary>
	/// <param name="path"> -p, Defaults to the`{pwd}/docs` folder</param>
	/// <param name="output"> -o, Defaults to `.artifacts/html` </param>
	/// <param name="pathPrefix"> Specifies the path prefix for urls </param>
	/// <param name="force"> Force a full rebuild of the destination folder</param>
	/// <param name="uploadToLinkIndex"> Upload the links.json file to the link index</param>
	/// <param name="ctx"></param>
	[Command("generate")]
	[ConsoleAppFilter<StopwatchFilter>]
	[ConsoleAppFilter<CatchExceptionFilter>]
	public async Task<int> Generate(
		string? path = null,
		string? output = null,
		string? pathPrefix = null,
		bool? force = null,
		bool uploadToLinkIndex = false,
		Cancel ctx = default
	)
	{
		pathPrefix ??= githubActionsService.GetInput("prefix");
		var fileSystem = new FileSystem();
		var context = new BuildContext(fileSystem, fileSystem, path, output)
		{
			UrlPathPrefix = pathPrefix,
			Force = force ?? false,
			Collector = new ConsoleDiagnosticsCollector(logger, githubActionsService),
		};
		var set = new DocumentationSet(context);
		var generator = new DocumentationGenerator(set, logger);
		await generator.GenerateAll(ctx);
		var linksJsonPath = set.LinkReferenceFile.FullName;
		await linkIndex.UploadFileAsync(linksJsonPath, uploadToLinkIndex);
		return context.Collector.Errors + context.Collector.Warnings;
	}

	/// <summary>
	/// Converts a source markdown folder or file to an output folder
	/// </summary>
	/// <param name="path"> -p, Defaults to the`{pwd}/docs` folder</param>
	/// <param name="output"> -o, Defaults to `.artifacts/html` </param>
	/// <param name="pathPrefix"> Specifies the path prefix for urls </param>
	/// <param name="force"> Force a full rebuild of the destination folder</param>
	/// <param name="uploadToLinkIndex"> Upload the links.json file to the link index</param>
	/// <param name="ctx"></param>
	[Command("")]
	[ConsoleAppFilter<StopwatchFilter>]
	[ConsoleAppFilter<CatchExceptionFilter>]
	public async Task<int> GenerateDefault(
		string? path = null,
		string? output = null,
		string? pathPrefix = null,
		bool? force = null,
		bool uploadToLinkIndex = false,
		Cancel ctx = default
	) =>
		await Generate(path, output, pathPrefix, force, uploadToLinkIndex, ctx);
}
