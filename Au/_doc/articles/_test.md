﻿# Markdown file

 
```csharp
var c = '\\'; //comment
var s=@"\\?\"; //comment
var s=@"\\?\ "; //comment
```

```csharp
var v = ADialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
var v = ADialog.ShowEx("", @"Text <a href=""example"">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
```

a `"code"` b

a | b
-|-
a `"code"` b | *city*

<table>
<tr>
<th>a</th>
<th>b</th>
</tr>
<tr>
<td>

a `"code"` b

</td>
<td>

*city*

</td>
</tr>
</table>

a <table>
<tr>
<th>a</th>
<th>b</th>
</tr>
<tr>
<td>a `"code"` b</td>
<td>*city*</td>
</tr>
</table>

<p>a `"code"` b</p>
a <p>a `"code"` b</p>


+---------+---------+
| Header  | Header  |
| Column1 | Column2 |
+=========+=========+
| 1. ab   | > This is a quote
| 2. cde  | > For the second column 
| 3. f    |
+---------+---------+
| Second row spanning
| on two columns
+---------+---------+
| Back    |         |
| to      |         |
| one     |         |
| column  |         | 

http://www.quickmacros.com

a http://www.quickmacros.com b

<!-- ![Video1](https://www.youtube.com/watch?v=mswPy5bt3TQ) -->

This is a test with a :) and a :angry: smiley
