/// To process markdown text use <link https://github.com/xoofx/markdig>Markdig<>. Install NuGet package <+nuget>Markdig<>.

/*/ nuget -\Markdig; /*/

var md = """
## Header
Text.
""";
var s = Markdig.Markdown.ToHtml(md);
print.it(s);
