using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers; using System.Windows.Forms; using System.Drawing;

partial class Script {

[Toolbars]
void WindowToolbars() {
	
	//Add toolbar triggers here, like in examples.
	
	if (_enableToolbarExamples) {
		//examples of toolbar triggers
		
		Triggers.Window[TWEvent.ActiveOnce, "*Notepad", "Notepad"] = Toolbar_Notepad;
		//Triggers.Window[TWEvent.ActiveOnce, "*Name1", "ClassName1"] = Toolbar_OtherWindow;
		//Triggers.Window[TWEvent.ActiveOnce, "*Name2", "ClassName2"] = o => { Toolbar_One(o); Toolbar_Two(o); } //attach 2 toolbars to this window
	}
}

//Add here your window-specific toolbars. Add their triggers in the above function.
//To create toolbars and add buttons can be used snippets. Start typing "tool" and you will see snippets in the completion list.



#region toolbar examples

void Toolbar_Notepad(WindowTriggerArgs ta = null) {
	var t = new AToolbar("Toolbar_Notepad");
	if (!t.SettingsModified) {
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
	
//	//auto-hide. Above is the auto-hide part. Below is the visible part.
//	t = t.AutoHide();
//	if(!t.SettingsModified) {
//		
//	}
	
	t.Show(ta);
}

//void Toolbar_OtherWindow(WindowTriggerArgs ta = null) { ... }

//void Toolbar_One(WindowTriggerArgs ta = null) { ... }

//void Toolbar_Two(WindowTriggerArgs ta = null) { ... }

#endregion

}
