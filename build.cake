#addin "MagicChunks"

var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var packPath            = Directory("./src/IdentityServer4.LinqToDB");
var sourcePath          = Directory("./src");
var testsPath           = Directory("test");
var buildArtifacts      = Directory("./artifacts/packages");
var solutionName        = "./IdentityServer4.LinqToDB.sln";

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{

	// Patch Version for CI builds
	if (!isLocalBuild)
	{
		var packageVersion = AppVeyor.Environment.Build.Version;
		var assemblyVersion = packageVersion + ".0";

		if (AppVeyor.Environment.Repository.Branch.ToLower() != "release")
			packageVersion = packageVersion + "-rc" + AppVeyor.Environment.Build.Number.ToString();

		Console.WriteLine("Package  Version: {0}", packageVersion);
		Console.WriteLine("Assembly Version: {0}", assemblyVersion);


		TransformConfig("./src/IdentityServer4.LinqToDB/IdentityServer4.LinqToDB.csproj", "./src/IdentityServer4.LinqToDB/IdentityServer4.LinqToDB.csproj",
		new TransformationCollection {
			{ "Project/PropertyGroup/Version",         packageVersion },
			{ "Project/PropertyGroup/AssemblyVersion", assemblyVersion },
			{ "Project/PropertyGroup/FileVersion",     assemblyVersion },
		 });

	}

	var settings = new DotNetCoreBuildSettings 
	{
		Configuration = configuration
		// Runtime = IsRunningOnWindows() ? null : "unix-x64"
	};

	DotNetCoreBuild(solutionName, settings); 
});

Task("RunTests")
	.IsDependentOn("Restore")
	.IsDependentOn("Clean")
	.Does(() =>
{
	var projects = GetFiles("./test/**/*.csproj");

	foreach(var project in projects)
	{
		var settings = new DotNetCoreTestSettings
		{
			Configuration = configuration
		};

		Console.WriteLine(project.FullPath);

		DotNetCoreTest(project.FullPath, settings);
	}
});

Task("Pack")
	.IsDependentOn("Restore")
	.IsDependentOn("Clean")
	.Does(() =>
{
	var settings = new DotNetCorePackSettings
	{
		Configuration = configuration,
		OutputDirectory = buildArtifacts,
		NoBuild = true
	};

	DotNetCorePack(packPath, settings);
});

Task("Clean")
	.Does(() =>
{
	CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

Task("Restore")
	.Does(() =>
{
	var settings = new DotNetCoreRestoreSettings
	{
		//Sources = new [] { "https://api.nuget.org/v3/index.json" }
	};

	DotNetCoreRestore(solutionName, settings);
});

Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("RunTests")
  .IsDependentOn("Pack");

RunTarget(target);