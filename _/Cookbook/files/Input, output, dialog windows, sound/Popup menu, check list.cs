/// To quickly insert <see cref="popupMenu"/> code, use snippet menuSnippet: in the code editor type menu and select from the list. To insert items can be used menuItemSnippet.
///
/// Simple menu. Function <see cref="popupMenu.showSimple"/> returns id of the selected item, or 0 if cancelled.

int i = popupMenu.showSimple("1 One|2 Two|3 Three||0 Cancel");
switch (i) {
case 1: print.it(1); break;
case 2: print.it(2); break;
case 3: print.it(3); break;
default: return;
}

/// Advanced menu.

var m = new popupMenu("4f14a87a-58e5-4bb6-96db-bbc5e6988e21");
m["A"] = o => { print.it(o); }; //executes code { print.it(o); } when clicked
m["B"] = o => {  };
m.Last.Tooltip = "Tooltip";
m.Submenu("", m => {
	m["C"] = o => {  };
	m["D"] = o => {  };
});
m.Separator();
m["Run program example"] = o => run.it(folders.System + @"notepad.exe");
m["Run script example"] = o => script.run("Script123456789.cs");
m["Copy-paste example"] = o => {
	string s = clipboard.copy();
	s = s.Upper();
	clipboard.paste(s);
};
m.Show();

/// Menu with checkbox items and radiobutton items.

var mm = new popupMenu("4ea61728-f775-4c55-b95c-7376a9243cc5");
mm.CheckDontClose = true;
var c1 = mm.AddCheck("A", true);
var c2 = mm.AddCheck("B", !true, o => { print.it(o); });
mm.Separator();
var r1 = mm.AddRadio("C", true);
var r2 = mm.AddRadio("D", !true);
mm.Separator();
mm.Add(1, "OK");
if (1 != mm.Show()) return;
print.it(c1.IsChecked, c2.IsChecked, r1.IsChecked, r2.IsChecked);
