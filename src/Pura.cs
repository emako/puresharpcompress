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

            if (lineTrimmed.StartsWith("namespace SharpCompress"))
            {
                isEdited = true;
                lines[i] = line.Replace("namespace SharpCompress", "namespace PureSharpCompress");
            }
            else if (lineTrimmed.StartsWith("using SharpCompress"))
            {
                isEdited = true;
                lines[i] = line.Replace("using SharpCompress", "using PureSharpCompress");
            }
            else if (lineTrimmed == "using Decoder = SharpCompress.Compressors.LZMA.RangeCoder.Decoder;")
            {
                isEdited = true;
                lines[i] = "using Decoder = PureSharpCompress.Compressors.LZMA.RangeCoder.Decoder;";
            }
            else if (lineTrimmed.StartsWith("using static SharpCompress"))
            {
                isEdited = true;
                lines[i] = line.Replace("using static SharpCompress", "using static PureSharpCompress");
            }
            else if (lineTrimmed.StartsWith("global using SharpCompress"))
            {
                isEdited = true;
                lines[i] = line.Replace("global using SharpCompress", "global using PureSharpCompress");
            }
            else if (lineTrimmed.Contains("new SharpCompress"))
            {
                isEdited = true;
                lines[i] = line.Replace("new SharpCompress", "new PureSharpCompress");
            }
            else if (lineTrimmed == "using ZstdSharp;")
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

            if (lineTrimmed.StartsWith("<TargetFrameworks>"))
            {
                isEdited = true;
                lines[i] = line.IndentStart() + Regex.Replace(lineTrimmed, @"<TargetFrameworks>(.*?)</TargetFrameworks>", $"<TargetFrameworks>{"net462;net472;net48;net481;netstandard2.0;net6.0;net8.0;net9.0;"}</TargetFrameworks>");
            }
            else if (lineTrimmed.StartsWith("<EmbedUntrackedSources>true</EmbedUntrackedSources>"))
            {
                isEdited = true;
                lines[i] += Environment.NewLine + line.IndentStart() + "<GeneratePackageOnBuild>true</GeneratePackageOnBuild>";
            }
            else if (lineTrimmed == "<PackageReference Include=\"ZstdSharp.Port\" />")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />-->";
            }
            else if (lineTrimmed == "<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<!--<PackageReference Include=\"Microsoft.Bcl.AsyncInterfaces\" />-->";
            }
            else if (lineTrimmed == "<ItemGroup Condition=\" '$(TargetFramework)' == 'netstandard2.0' \">")
            {
                isEdited = true;
                lines[i] = lineTrimmed.Replace("<ItemGroup Condition=\" '$(TargetFramework)' == 'netstandard2.0' \">",
                """
                  <ItemGroup Condition=" '$(VersionlessImplicitFrameworkDefine)' == 'NETFRAMEWORK' ">
                    <!--<PackageReference Include="Microsoft.Bcl.AsyncInterfaces" />-->
                    <PackageReference Include="System.Text.Encoding.CodePages"  />
                    <PackageReference Include="System.Memory"  />
                  </ItemGroup>
                  <ItemGroup Condition=" '$(TargetFramework)' == 'netstandard2.0' ">
                """);
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
                lines[i] = line.IndentStart() + "<PackageId>PureSharpCompress</PackageId>"
                    + Environment.NewLine + line.IndentStart() + "<AssemblyName>PureSharpCompress</AssemblyName>"
                    + Environment.NewLine + line.IndentStart() + "<RootNamespace>PureSharpCompress</RootNamespace>";
            }
            else if (lineTrimmed == "<AssemblyName>SharpCompress</AssemblyName>")
            {
                isEdited = true;
                lines[i] = line.IndentStart() + "<AssemblyName>PureSharpCompress</AssemblyName>";
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
                    + Environment.NewLine + line.IndentStart() + "<Version>$(VersionPrefix)</Version>";
            }
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
            else if (lineTrimmed.StartsWith("<Description>"))
            {
                isEdited = true;
                lines[i] = line.Replace("SharpCompress", "PureSharpCompress");
            }
            else if (lineTrimmed.StartsWith("<AssemblyTitle>"))
            {
                isEdited = true;
                lines[i] = line.Replace("SharpCompress", "PureSharpCompress");
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
