# Markdown file

 
```csharp
var c = '\\'; //comment
var s=@"\\?\"; //comment
var s=@"\\?\ "; //comment
```

```csharp
var v = AuDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
var v = AuDialog.ShowEx("", @"Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
```
