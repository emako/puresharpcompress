using System.Reflection;
using System.Text.RegularExpressions;

internal static class Pura
{
    public static void PuraIt(string filePath)
    {
        FileInfo fileInfo = new(filePath);

        if (fileInfo.Extension.Equals(".cs", StringComparison.CurrentCultureIgnoreCase))
        {
            PuraCs(filePath);
        }
        else if (fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
        {
            PuraCsproj(filePath);
        }
    }

    public static void PuraCs(string filePath)
    {
        bool isEdited = false;
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string lineTrimmed = line.Trim();

            if (lineTrimmed == "using ZstdSharp;")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "// " + line;
            }
            else if (lineTrimmed == "return new DecompressionStream(stream);"
                  || lineTrimmed == "return new DecompressionStream(inStreams.Single());")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "return default!;";
            }
        }

        if (isEdited)
        {
            File.WriteAllLines(filePath, lines);
        }
    }

    public static void PuraCsproj(string filePath)
    {
        bool isEdited = false;
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string lineTrimmed = line.Trim();

            if (lineTrimmed == "<PackageReference Include=\"ZstdSharp.Port\" />")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />-->";
            }
            else if (lineTrimmed == "<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />-->";
            }
            else if (lineTrimmed == "<PackageProjectUrl>https://github.com/adamhathcock/sharpcompress</PackageProjectUrl>")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<PackageProjectUrl>https://github.com/emako/puresharpcompress</PackageProjectUrl>"
                    + Environment.NewLine + line.IndentStart() + "<RepositoryUrl>https://github.com/emako/puresharpcompress</RepositoryUrl>";
            }
            else if (lineTrimmed == "<PackageId>SharpCompress</PackageId>")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<PackageId>PureSharpCompress</PackageId>";
            }
            else if (lineTrimmed.StartsWith("<VersionPrefix>"))
            {
                isEdited = true;
                string version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimmed, @"<VersionPrefix>(.*?)</VersionPrefix>", $"<VersionPrefix>{version}</VersionPrefix>");
            }
            else if (lineTrimmed.StartsWith("<AssemblyVersion>"))
            {
                isEdited = true;
                string version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimmed, @"<AssemblyVersion>(.*?)</AssemblyVersion>", $"<AssemblyVersion>{version}</AssemblyVersion>");
            }
            else if (lineTrimmed.StartsWith("<FileVersion>"))
            {
                isEdited = true;
                string version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimmed, @"<FileVersion>(.*?)</FileVersion>", $"<FileVersion>{version}</FileVersion>")
                    + Environment.NewLine + line.IndentStart() + "<Version>$(VersionPrefix)-rc2</Version>";
            }
            //else if (lineTrimmed.StartsWith("<AssemblyOriginatorKeyFile>../../SharpCompress.snk</AssemblyOriginatorKeyFile>"))
            //{
            //    isEdited = true;
            //    lines[i] = line.IndentStart() + "<!--<AssemblyOriginatorKeyFile>../../SharpCompress.snk</AssemblyOriginatorKeyFile>-->";
            //}
            //else if (lineTrimmed.StartsWith("<SignAssembly>true</SignAssembly>"))
            //{
            //    isEdited = true;
            //    lines[i] = line.IndentStart() + "<SignAssembly>false</SignAssembly>";
            //}
            else if (lineTrimmed.StartsWith("<None Include=\"..\\..\\README.md\" Pack=\"true\" PackagePath=\"\\\" />"))
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<None Include=\"..\\..\\..\\README.md\" Pack=\"true\" PackagePath=\"\\\" />"
                    + Environment.NewLine + line.IndentStart() + "<None Include=\"..\\..\\..\\branding\\logo.png\" Pack=\"true\" PackagePath=\"\\\" />";
            }
            else if (lineTrimmed.StartsWith("<PackageReadmeFile>README.md</PackageReadmeFile>"))
            {
                isEdited = true;
                lines[i] = line.IndentStart() + lineTrimmed
                    + Environment.NewLine + line.IndentStart() + "<PackageIcon>logo.png</PackageIcon>";
            }
        }

        if (isEdited)
        {
            File.WriteAllLines(filePath, lines);
        }
    }

    private static string IndentStart(this string line)
    {
        if (line.Contains('\t'))
        {
            return new string('\t', line.TakeWhile(c => c == '\t').Count());
        }
        else
        {
            return new string(' ', line.TakeWhile(c => c == ' ').Count());
        }
    }
}
