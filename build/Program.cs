using ConsoleAppFramework;
using ProcNet;
using Zx;
//using static Zx.Env;

var app = ConsoleApp.Create();
app.Add("", async (Cancel _) =>
{
	await "dotnet tool restore";
	await "dotnet build -c Release --verbosity minimal";
});
app.Add("validate-licenses", (Cancel _) =>
{
	string[] args =
	[
		"-u", "-t", "-i", "docs-builder.sln", "--use-project-assets-json",
		"--forbidden-license-types", "build/forbidden-license-types.json",
		"--packages-filter", "#System..*#"
	];
	Proc.Exec("dotnet", ["dotnet-project-licenses", ..args]);
});

app.Add("publish", async (Cancel _) =>
{
	var source = "src/docs-builder/docs-builder.csproj";
	await $"""
		dotnet publish {source} -c Release -o .artifacts/publish \
			--self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=false /p:PublishAot=true
		""";
});
await app.RunAsync(args);
