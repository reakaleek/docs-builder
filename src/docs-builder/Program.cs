using ConsoleAppFramework;
using Documentation.Builder.Cli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(x =>
{
	x.ClearProviders();
	x.SetMinimumLevel(LogLevel.Information);
	x.AddSimpleConsole(c =>
	{
		c.SingleLine = true;
		c.IncludeScopes = true;
		c.UseUtcTimestamp = true;
		c.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss] ";
	});
});

await using var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
ConsoleApp.ServiceProvider = serviceProvider;
ConsoleApp.Log = msg => logger.LogInformation(msg);
ConsoleApp.LogError = msg => logger.LogError(msg);

var app = ConsoleApp.Create();
app.Add<Commands>();

await app.RunAsync(args);
