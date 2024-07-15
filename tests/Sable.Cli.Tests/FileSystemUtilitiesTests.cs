// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.Runtime.InteropServices;
using Sable.Cli.Utilities;
using Xunit;

namespace Sable.Cli.Tests;

public class FileSystemUtilitiesTests
{
    [Fact]
    public Task EnsureProjectDirectoryIsResolvedCase1()
    {
        var projectFilePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"C:\Users\sable\dev\Sable\Sable.csproj"
            : "/home/sable/dev/Sable/Sable.csproj";
        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(projectFilePath);
        var expectedProjectDirectory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"C:\Users\sable\dev\Sable"
            : "/home/sable/dev/Sable";
        Assert.Equal(projectDirectory, expectedProjectDirectory);
        return Task.CompletedTask;
    }

    [Fact]
    public Task EnsureProjectDirectoryIsResolvedCase2()
    {
        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(null);
        var expectedProjectDirectory = Directory.GetCurrentDirectory();
        Assert.Equal(projectDirectory, expectedProjectDirectory);
        return Task.CompletedTask;
    }
}
