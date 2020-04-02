using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {

/// <summary>
/// Attaches toolbars to Notepad windows.
/// </summary>
[Toolbars]
void NotepadToolbars() {
	Triggers.Window[TWEvent.ActiveOnce, "*Notepad", "Notepad"] = o => _Toolbar_Notepad(o.Window);
	//Triggers.Window[TWEvent.ActiveOnce, "*Notepad2", "Notepad2"] = o => _Toolbar_Notepad2(o.Window);
	//...
}

void _Toolbar_Notepad(AWnd w) {
	var t = new AToolbar("_Toolbar_Notepad");
	if(!t.SettingsModified) {
		t.AutoSize = true;
	}
	
	t[""] = o => {  };
	t[""] = o => {  };
	t.MenuButton("", m => {
		m[""] = o => {  };
		m[""] = o => {  };
	});
	t.Separator();
	t[""] = o => {  };
	t[""] = o => {  };
	
	t.Show(w);
}

//void _Toolbar_Notepad2() { ... }

//...

//To create toolbars and add buttons can be used snippets. Start typing "toolbar" and you will see snippets in the completion list.

}
