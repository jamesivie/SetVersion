using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SetVersion
{
    class Program
    {
        private static Regex VersionNumberValidate = new Regex(@"^[0-9]+(\.[0-9]+){1,3}$", RegexOptions.Compiled);
        private static Regex ProjectFileVersionReplace = new Regex(@"(?<=<.*Version>)[0-9]+(\.([0-9]+|\*)){1,3}", RegexOptions.Compiled);
        private static Regex AssemblyInfoVersionReplace = new Regex(@"(?<=\[.*Assembly.*Version ?\(\"")([0-9]+(?:\.(?:[0-9]+|\*)){1,3})", RegexOptions.Compiled);
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("\tSetVersion <version_number>");
                Console.WriteLine("");
                return;
            }
            // validate the specified version number
            if (!VersionNumberValidate.IsMatch(args[1]))
            {
                Console.WriteLine("The specified version must be of the form nnn.nnn[.nnn[.nnn]]");
                Console.WriteLine("");
                return;
            }
            foreach (string targetFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj", SearchOption.AllDirectories))
            {
                UpdateFile(targetFile, args[1]);
            }
            foreach (string targetFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "AssemblyInfo.cs", SearchOption.AllDirectories))
            {
                UpdateFile(targetFile, args[1]);
            }
        }

        private static void UpdateFile(string targetFile, string newVersion)
        {
            string targetFileContents = File.ReadAllText(targetFile);
            switch (Path.GetExtension(targetFile).ToUpperInvariant())
            {
                case ".CSPROJ":
                    targetFileContents = ProjectFileVersionReplace.Replace(targetFileContents, newVersion);
                    break;
                case ".CS":
                    targetFileContents = AssemblyInfoVersionReplace.Replace(targetFileContents, newVersion);
                    break;
                default:
                    break;
            }
            File.WriteAllText(targetFile, targetFileContents);
            Console.WriteLine($"Updated versions in {targetFile} to {newVersion}.");
        }
    }
}
