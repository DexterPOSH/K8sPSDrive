// Currently there is an issue with below Cake.PowerShell, it downloads huge sum of dependencies
// so have to specify loaddependencies true -> https://github.com/SharpeRAD/Cake.Powershell/issues/84 
// #addin nuget:?package=Cake.Powershell&version=1.0.1&loaddependencies=true

using System.Collections;
using System.Runtime.InteropServices;


///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var civersion = Argument<string>("civersion", "0.0.1");

System.Console.WriteLine($"Version: {civersion}");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projects = GetFiles("./**/*.csproj");
var sourceProjectPath = GetFiles("./Src/*.csproj").FirstOrDefault();
var pesterTestFiles = GetFiles("./Tests/*.Tests.ps1");

var projectPaths = projects.Select(project => project.GetDirectory().ToString());
var artifactsDir = "./ci/artifacts";
var publishDir = "./ci/publish/dotnet";
var coverageDir = MakeAbsolute(Directory("./ci/coverage"));


string runtime;
string pwshExeName;
if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    runtime = "linux-x64";
    pwshExeName = "pwsh";
}
if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    runtime = "win-x64";
    pwshExeName = "pwsh.exe";
}
if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    runtime = "osx-x64";
    pwshExeName = "pwsh";
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Running tasks...");
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    var settings = new DeleteDirectorySettings {
        Recursive = true,
        Force = true
    };
    // Clean solution directories.
    foreach(var path in projectPaths)
    {
        Information($"Cleaning path {path} ...");
        var directoriesToDelete = new DirectoryPath[]{
            Directory($"{path}/obj"),
            Directory($"{path}/bin")
        };
        foreach(var dir in directoriesToDelete)
        {
            if (DirectoryExists(dir))
            {
                DeleteDirectory(dir, settings);
            }
        }
    }
    // Delete artifact output too
    if (DirectoryExists(artifactsDir))
    {
        Information($"Cleaning path {artifactsDir} ...");
        DeleteDirectory(artifactsDir, settings);
    }
    if (DirectoryExists(publishDir))
    {
        Information($"Cleaning path {publishDir} ...");
        DeleteDirectory(publishDir, settings);
    }
    if (DirectoryExists(coverageDir))
    {
        Information($"Cleaning path {coverageDir} ...");
        DeleteDirectory(coverageDir, settings);
    }
});

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var path in projectPaths)
    {
        Information($"Restoring {path}...");
        DotNetCoreRestore(path);
    }
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Framework = "netstandard2.0",
        Configuration = configuration,
        OutputDirectory = artifactsDir,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetVersion(civersion)
            .SetFileVersion(civersion)
    };
    Information($"{sourceProjectPath.FullPath}");
    DotNetCoreBuild(sourceProjectPath.FullPath, settings);
});

Task("Publish")
    .Description("Publishes the the project.")
    .Does(() =>
{
    var settings = new DotNetCorePublishSettings
    {
        Framework = "netstandard2.0",
        Configuration = configuration,
        OutputDirectory = publishDir,
        Runtime = runtime,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetVersion(civersion)
            .SetFileVersion(civersion)
    };

     DotNetCorePublish(sourceProjectPath.FullPath, settings);
});

///////////////////////////////////////////////////////////////////////////////
// Tests
///////////////////////////////////////////////////////////////////////////////

Task("Test")
    .Description("Run Pester Tests.")
    //.IsDependentOn("Build")
    .Does(() =>
{
    // Bootstrap required modules for PowerShell
    using(var process = StartAndReturnProcess(pwshExeName, new ProcessSettings{ Arguments = "-File ./build.ps1 -BootStrapOnly"}))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        //var output = String.Join(' ', process.GetStandardOutput());
        //Information(output);
        Information("Exit code: {0}", process.GetExitCode());
    }

   foreach (var pesterTestFile in pesterTestFiles)
   {
        // run tests
        using(var process = StartAndReturnProcess(pwshExeName, new ProcessSettings{ Arguments = $"-File {pesterTestFile.FullPath} -ModulePath {artifactsDir}"}))
        {
            process.WaitForExit();
            // This should output 0 as valid arguments supplied
            //var output = String.Join(' ', process.GetStandardOutput());
            //Information(output);
            Information("Exit code: {0}", process.GetExitCode());
        }
        
   }
});



///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("This is the default task which will be ran if no specific target is passed in.")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
