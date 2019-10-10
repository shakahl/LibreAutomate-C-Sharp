# Code editor

C# code may look like this:
```csharp
AMouse.Click(10, 20);
if (AKeys.IsCtrl) {
	Print("text");
}
```

The code editor helps you to type it. You can type this text instead:
```csharp
amo.c 10, 20
if ak.isct

pr "text";
```

While typing, editor completes words, adds `()`, `;`, `{}` and indentation. The result is the first code.

#### The list of symbols and autocompletion
When you start typing a word, editor shows a list of symbols (classes, functions, C# keywords, etc) available there. Or you can press Ctrl+Space to show the list anywhere, including regular expression strings.

The list shows only items that contain the partially typed text. The text in list items is highlighted. Auto-selects an item that is considered the first best match. On Ctrl+Space shows all. You can use the vertical toolbar to filter and group list items, for example to show only methods.

While the list is visible, when you enter a non-word character (space, comma, etc) or press Enter or Tab or double-click an item, the partially or fully typed word is replaced with the text of the selected list item. Also may add `()` or `{}`, for example if it is a method or a keyword like `if` or `finally`. The added text may be different when you press Enter; for example may add `{}` instead of `()` when both are valid after that keyword.

To select list items you also can click or press arrow or page keys. It does not hide the list. The tooltip-like window next to the list shows more info about the selected item, including links to online documentation and source code if available. Shows all overloads (functions with same name but different parameters), and you can click them to view their info.

To hide the list without inserting item text you can press Esc or click somewhere in code.

#### Automatic brace completion
When you type (, [, {, <, " or ', editor adds the closing ), ], }, >, " or '.