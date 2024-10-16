using System.IO.Abstractions;
using ConsoleAppFramework;
using Documentation.Builder.Http;
using Elastic.Markdown;
using Microsoft.Extensions.Logging;

namespace Documentation.Builder.Cli;

internal class Commands(ILoggerFactory logger)
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
	}

	/// <summary>
	/// Converts a source markdown folder or file to an output folder
	/// </summary>
	/// <param name="path"> -p, Defaults to the`{pwd}/docs` folder</param>
	/// <param name="output"> -o, Defaults to `.artifacts/html` </param>
	/// <param name="ctx"></param>
	[Command("generate")]
	[ConsoleAppFilter<StopwatchFilter>]
	[ConsoleAppFilter<CatchExceptionFilter>]
	public async Task Generate(string? path = null, string? output = null, Cancel ctx = default)
	{
		var generator = DocumentationGenerator.Create(path, output, logger, new FileSystem());
		await generator.GenerateAll(ctx);
	}

	/// <summary>
	/// Converts a source markdown folder or file to an output folder
	/// </summary>
	/// <param name="path"> -p, Defaults to the`{pwd}/docs` folder</param>
	/// <param name="output"> -o, Defaults to `.artifacts/html` </param>
	/// <param name="ctx"></param>
	[Command("")]
	[ConsoleAppFilter<StopwatchFilter>]
	[ConsoleAppFilter<CatchExceptionFilter>]
	public async Task GenerateDefault(string? path = null, string? output = null, Cancel ctx = default) =>
		await Generate(path, output, ctx);
}
