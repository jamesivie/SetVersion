using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SetVersion
{
    class Program
    {
        private static Regex VersionNumberValidate = new Regex(@"^[0-9]+(\.[0-9]+){1,3}$", RegexOptions.Compiled);
        private static Regex ProjectFileVersionReplace = new Regex(@"(?<=<(?:Assembly|File|Package|)Version>)[^<]*", RegexOptions.Compiled);
        private static Regex PackageOutputPathReplace = new Regex(@"<PackageOutputPath>[^<]*</PackageOutputPath>", RegexOptions.Compiled);
        private static Regex AssemblyInfoVersionReplace = new Regex(@"(?<=\[.*Assembly.*Version ?\(\"")([^\""]*)", RegexOptions.Compiled);
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("\tSetVersion <version_number>");
                Console.WriteLine("");
                return;
            }
            string version = null;
            bool noPackageOutputPath = false;
            foreach (string arg in args)
            {
                if (String.Equals(arg, "-NoPackageOutputPath") || String.Equals(arg, "/NoPackageOutputPath")) noPackageOutputPath = true;
                // check for a version number
                if (VersionNumberValidate.IsMatch(arg)) version = arg;
            }
            if (version == null)
            {
                Console.WriteLine("The specified version must be of the form nnn.nnn[.nnn[.nnn]]");
                Console.WriteLine("");
                return;
            }
            foreach (string targetFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj", SearchOption.AllDirectories))
            {
                UpdateProjectFile(targetFile, args[0], noPackageOutputPath);
            }
            foreach (string targetFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "AssemblyInfo.cs", SearchOption.AllDirectories))
            {
                UpdateCodeFile(targetFile, args[0]);
            }
        }

        private static void UpdateProjectFile(string targetFile, string newVersion, bool noPackageOutputPath)
        {
            string targetFileContents = File.ReadAllText(targetFile);
            targetFileContents = ProjectFileVersionReplace.Replace(targetFileContents, newVersion);
            if (noPackageOutputPath) targetFileContents = PackageOutputPathReplace.Replace(targetFileContents, "");
            File.WriteAllText(targetFile, targetFileContents);
            Console.WriteLine($"Updated versions in {targetFile} to {newVersion}.");
        }
        private static void UpdateCodeFile(string targetFile, string newVersion)
        {
            string targetFileContents = File.ReadAllText(targetFile);
            targetFileContents = AssemblyInfoVersionReplace.Replace(targetFileContents, newVersion);
            File.WriteAllText(targetFile, targetFileContents);
            Console.WriteLine($"Updated versions in {targetFile} to {newVersion}.");
        }
    }
}
