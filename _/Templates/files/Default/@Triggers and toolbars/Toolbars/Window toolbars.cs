using Au.Triggers;

partial class Program {

[Toolbars]
void WindowToolbars() {
	
	//Add toolbar triggers here, like in examples.
	
	if (!true) { //examples of triggers. To enable, replace (!true) with (true) and run this script.
		Triggers.Window[TWEvent.ActiveOnce, "*Notepad", "Notepad"] = Toolbar_Notepad;
		//Triggers.Window[TWEvent.ActiveOnce, "*Name1", "ClassName1"] = Toolbar_OtherWindow;
		//Triggers.Window[TWEvent.ActiveOnce, "*Name2", "ClassName2"] = o => { Toolbar_One(o); Toolbar_Two(o); } //attach 2 toolbars to this window
	}
}

//Add here your window-specific toolbars. Add their triggers in the above function.
//To create toolbars and add buttons can be used snippets. Start typing "tool" and you will see snippets in the completion list.
//Click the Run button to apply changes after editing.



#region toolbar examples

void Toolbar_Notepad(WindowTriggerArgs ta = null) {
	var t = new toolbar("Toolbar_Notepad");
	if (t.FirstTime) {
		
	}
	
	t[""] = o => {  };
	t[""] = o => {  };
	t.Menu("", m => {
		m[""] = o => {  };
		m[""] = o => {  };
	});
	t.Separator();
	t[""] = o => {  };
	t[""] = o => {  };
	
//	//auto-hide. Above is the auto-hide part. Below is the visible part.
//	t = t.AutoHide();
//	if(t.FirstTime) {
//		
//	}
	
	t.Show(ta);
}

//void Toolbar_OtherWindow(WindowTriggerArgs ta = null) { ... }

//void Toolbar_One(WindowTriggerArgs ta = null) { ... }

//void Toolbar_Two(WindowTriggerArgs ta = null) { ... }

#endregion

}
