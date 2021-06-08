---
uid: caller_info
---

# Caller info parameter

Some functions have parameters f_ and/or l_ with attribute [CallerFilePath] or [CallerLineNumber]. When calling, they should not be used. They automatically pass source file/line info to the function.

For example, menus (class popupMenu) and toolbars (class toolbar) use it for the "Edit" command of their context menu. Triggers use it for menu Run -> Recent -> trigger.

If to add menu/toolbar items or triggers you create an intermediate function, the function should have f_ and/or l_ parameters and pass them to the menu/toolbar/trigger function. Example script:

```csharp
using Au;
using System;
using System.Runtime.CompilerServices;

var m = new popupMenu("431beeab-e190-495a-be8f-05732ef6a74f");
m["Right click me to edit"] = o => print.it(1);
_AddMenuItem("You can edit me too", o => print.it(2));
_AddMenuItemWrongWay("But can be difficult to edit me", o => print.it(3));
m.Show();

void _AddMenuItem(string label, Action<popupMenu.MenuItem> action, [CallerLineNumber] int l_ = 0) {
	m[label, l_: l_] = action;
	m.Last.TextColor = 0x0000ff;
}

void _AddMenuItemWrongWay(string label, Action<popupMenu.MenuItem> action) {
	m[label] = action;
	m.Last.TextColor = 0xff0000;
}
```