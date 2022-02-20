using Cake.Core.IO;

namespace Publish.Models;

public record NugetPackage(string PackageName, FilePath FilePath);
