# Markdown file

 
```csharp
var c = '\\'; //comment
var s=@"\\?\"; //comment
var s=@"\\?\ "; //comment
```

```csharp
var v = AuDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
var v = AuDialog.ShowEx("", @"Text <a href=""example"">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
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
<td>a `"code"` b</td>
<td>*city*</td>
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
