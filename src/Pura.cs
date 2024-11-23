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
            string lineTrimed = line.Trim();

            if (lineTrimed == "using ZstdSharp;")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "// " + line;
            }
            else if (lineTrimed == "return new DecompressionStream(stream);"
                  || lineTrimed == "return new DecompressionStream(inStreams.Single());")
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
            string lineTrimed = line.Trim();

            if (lineTrimed == "<PackageReference Include=\"ZstdSharp.Port\" />")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />-->";
            }
            else if (lineTrimed == "<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />-->";
            }
            else if (lineTrimed == "<PackageProjectUrl>https://github.com/adamhathcock/sharpcompress</PackageProjectUrl>")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<PackageProjectUrl>https://github.com/emako/sharpcompress.pure</PackageProjectUrl>";
            }
            else if (lineTrimed == "<PackageId>SharpCompress</PackageId>")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<PackageId>PureSharpCompress</PackageId>";
            }
            else if (lineTrimed.StartsWith("<VersionPrefix>"))
            {
                isEdited = true;
                string version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimed, @"<VersionPrefix>(.*?)</VersionPrefix>", $"<VersionPrefix>{version}</VersionPrefix>");
            }
            else if (lineTrimed.StartsWith("<AssemblyVersion>"))
            {
                isEdited = true;
                string version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimed, @"<AssemblyVersion>(.*?)</AssemblyVersion>", $"<AssemblyVersion>{version}</AssemblyVersion>");
            }
            else if (lineTrimed.StartsWith("<FileVersion>"))
            {
                isEdited = true;
                string version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimed, @"<FileVersion>(.*?)</FileVersion>", $"<FileVersion>{version}</FileVersion>");
            }
            else if (lineTrimed.StartsWith("<AssemblyOriginatorKeyFile>../../SharpCompress.snk</AssemblyOriginatorKeyFile>"))
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<AssemblyOriginatorKeyFile>../../SharpCompress.snk</AssemblyOriginatorKeyFile>-->";
            }
            else if (lineTrimed.StartsWith("<SignAssembly>true</SignAssembly>"))
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<SignAssembly>false</SignAssembly>";
            }
            else if (lineTrimed.StartsWith("<None Include=\"..\\..\\README.md\" Pack=\"true\" PackagePath=\"\\\" />"))
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<None Include=\"..\\..\\..\\README.md\" Pack=\"true\" PackagePath=\"\\\" />";
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
