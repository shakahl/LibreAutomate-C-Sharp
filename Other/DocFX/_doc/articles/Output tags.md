---
uid: output_tags
---

# Output tags

Function [AOutput.Write]() supports links, colors, images, etc. For it use tags in text. Similar to HTML tags. Also text must start with `<>`. It works only when text is displayed in the editor's output pane, not when in console.

For most tags use this format: `<tag>text<>` or `<tag attribute>text<>`.

For these tags use `<tag>text</tag>`: `<code>`, `<_>`, `<\a>`, `<fold>`.

This tag does not have a closing tag: `<image "attribute">`.

Attribute can be enclosed in `'` or `"`. If attribute omitted, text is used as attribute if need for that tag.

Tags can be nested, like `<b><c green>text<><>` or `<b>text <c green>text<> text<>`.

##### Examples

```csharp
AOutput.Write("<>Text <i>italic<>, <c green>color<>, <link http://www.example.com>Link<>.");
AOutput.Write("<>Code example:\r\n<code>Mouse.Click(10, 20); //comments</code>");
```

#### Simple formatting tags
| Examples | Comments
| - | -
| `<b>text<>` | Bold text.
| `<i>text<>` | Italic text.
| `<bi>text<>` | Bold italic.
| `<u>text<>` | Underline.
| `<c 0xE0A000>text<>`<br/>`<c #E0A000>text<>`<br/>`<c green>text<>` | Text color.<br/>Can be 0xRRGGBB, #RRGGBB or .NET color name.
| `<z yellow>text<>` | Text background color.
| `<Z wheat>line text<>` | Line background color.
| `<size 10>text<>` |  Font height.<br/>Note: it can increase height of all lines.
| `<mono>text<>` | Monospace font.

#### Links
| Examples | Comments
| - | -
| `<link http://www.example.com>text<>`<br/>`<link C:\files\example.exe>text<>`<br/>`<link>http://www.example.com<>`<br/>`<link>C:\files\example.exe<>`<br/>`<link C:\example.exe|args>text<>` | Opens a web page or runs a program, file, folder.<br/>Calls function [AExec.TryRun]().
| `<explore>C:\files\example<>` | Selects a file or folder in File Explorer.<br/>Calls function [AExec.SelectInExplorer]().
| `<google s1>text<>`<br/>`<google>s1<>`<br/>`<google s1|s2>text<>` | Google. Opens this URL:<br/>`$"http://www.google.com/search?q={s1}{s2}"`<br/>Don't need to URL-encode.
| `<help>Class.Function<>`<br/>`<help Au.Namespace.Class>text<>`<br/>`<help articles/Output tags>text<>` | Opens a help page of this library.
| `<open>Script5.cs<>`<br/>`<open \Folder\Script5.cs>text<>`<br/>`<open Script5.cs|10>text<>`<br/>`<open Script5.cs|10|15>text<>` | Opens a script or other file in the code editor.<br/>The file must be in the workspace.<br/>The 10 is 1-based line index.<br/>The 15 is 1-based character index in line.
| `<script>Script5.cs<>`<br/>`<script \Folder\Script5.cs>text<>`<br/>`<script Script5.cs|args0|args1>text<>` | Runs a script.

#### Other tags
| Examples | Comments
| - | -
| `<_>text</_>` or `<\a>text</\a>` | Literal text. Tags in it are ignored.<br/>Here `\a` is escape sequence for character code 7.
| `<code>var s="example";</code>` | Colored C# code. Tags in it are ignored.
| `<fold>text</fold>` | Folded (hidden) lines. Adds a link to unfold (show).

#### Images
Images are displayed below current line. Examples:

`<image "c:\images\example.png">`
`<image "c:\files\example.txt">`
`<image "resource:ResourceName">`
`<image "image:PngBase64">`

Supports images of formats: png, bmp, jpg, gif, ico (only 16x16). For other file types and folders displays small file icon.

Supports managed image resources of the entry assembly; not icons.

Supports Base64-encoded png image file data. To create such string use dialog "Find image or color in window" or function **Au.Controls.ImageUtil.ImageToString** (in Au.Controls.dll).
