#tool "nuget:?package=GitVersion.CommandLine"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var verbosity = Argument<string>("verbosity", "Diagnostic");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var artifactsDir = "./artifacts/";
var publishDir = "./publish/";

var solutionPath = "./website-performance.sln";
var projectPath = "./src/website-performance/website-performance.csproj";

var unitTestProjects = GetFiles("./tests/*.Tests.Unit/*.Tests.Unit.csproj");
var acceptanceTestProjects = GetFiles("./tests/*.Tests.Acceptance/*.Tests.Acceptance.csproj");
GitVersion gitVersion = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    // Executed BEFORE the first task.
    EnsureDirectoryExists(artifactsDir);
    EnsureDirectoryExists(publishDir);
    Verbose("Running tasks...");
});

Teardown(ctx =>
{
    // Executed AFTER the last task.
    Verbose("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// HELPERS
///////////////////////////////////////////////////////////////////////////////

string GetNuGetApiKey()
{
  return GetSetting("nugetapikey");
}

string GetNuGetApiUrl()
{
  return GetSetting("nugetapiurl", "https://www.nuget.org/api/v2/package");
}

string GetSetting(string key, string defaultValue = "")
{
  return EnvironmentVariable("cake-" + key) ?? Argument<string>(key, defaultValue);
}

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() => {
		CleanDirectories("./src/**/bin/**");
		CleanDirectories("./src/**/obj/**");
		CleanDirectories("./tests/**/bin/**");
		CleanDirectories("./tests/**/obj/**");
		CleanDirectory(artifactsDir);
		CleanDirectory(publishDir);
    });

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the solution.")
	.Does(() => {
	    var settings = new DotNetCoreRestoreSettings
		{
			Sources = new[]
			{
				"https://api.nuget.org/v3/index.json"
			},
			DisableParallel = false
		};

        DotNetCoreRestore("./", settings);
    });

Task("GitVersion")
    .Description("Set the SemVer to the solution.")
    .Does(() =>
	{
		try
		{
			gitVersion = GitVersion(new GitVersionSettings 
			{
				UpdateAssemblyInfo = true
			});

			var file = File(projectPath);
			XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", gitVersion.MajorMinorPatch);
			XmlPoke(file, "/Project/PropertyGroup/FileVersion", gitVersion.MajorMinorPatch);
			XmlPoke(file, "/Project/PropertyGroup/PackageVersion", gitVersion.MajorMinorPatch);
			XmlPoke(file, "/Project/PropertyGroup/Version", gitVersion.FullSemVer);

			Verbose("Full SemVer: " + gitVersion.FullSemVer);
			Verbose("Major Minor Patch: " + gitVersion.MajorMinorPatch);
		}
		catch(Exception ex)
		{
			// TODO: Travis CI fail here. Perhaps Cake problems. Investigate later.
			Verbose(ex.Message);
		}
	});

Task("Build")
    .Description("Builds the solution.")
    .IsDependentOn("GitVersion")
    .Does(() => {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = configuration
		};

        DotNetCoreBuild(solutionPath, settings);
    });

Task("Unit-Tests")
    .Description("Runs the Unit Tests.")
    .Does(() => {
	    var settings = new DotNetCoreTestSettings
		{
			Configuration = configuration,
			NoBuild = true
		};

		foreach(var testProject in unitTestProjects)
        {
			Verbose("Running tests in project " + testProject.FullPath);
			DotNetCoreTest(testProject.FullPath, settings);
		}
    });

Task("Acceptance-Tests")
    .Description("Runs the Acceptance Tests.")
    .Does(() => {
	    var settings = new DotNetCoreTestSettings
		{
			Configuration = configuration,
			NoBuild = true
		};

		foreach(var testProject in acceptanceTestProjects)
        {
			Verbose("Running tests in project " + testProject.FullPath);
			DotNetCoreTest(testProject.FullPath, settings);
		}
    });

Task("Publish")
    .Description("Publish the Web Application.")
    .Does(() => {
	    var settings = new DotNetCorePublishSettings
		{
			Configuration = configuration,
			OutputDirectory = publishDir
		};

        DotNetCorePublish(projectPath, settings);
    });

Task("Package")
    .Description("Package the Web Application.")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Test")
	.IsDependentOn("Publish")
	.Does(() => {
		var settings = new DotNetCorePackSettings
		{
		    Configuration = configuration,
			OutputDirectory = artifactsDir
		};

		DotNetCorePack(projectPath, settings);
	});

Task("PushPackage")
    .Description("Pushes the Web Application package.")
	.IsDependentOn("Package")
	.Does(() => {
		var packages = GetFiles(artifactsDir + "*.nupkg");

		var settings = new DotNetCoreNuGetPushSettings
        {
			Source = GetNuGetApiUrl(),
			ApiKey = GetNuGetApiKey()
		};
        
        foreach(var package in packages)
        {
		    DotNetCoreNuGetPush(package.FullPath, settings);
        }
	});

Task("Test")
    .IsDependentOn("Unit-Tests")
    .IsDependentOn("Acceptance-Tests");

Task("Build+Test")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("This is the default task which will be ran if no specific target is passed in.")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Test");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
