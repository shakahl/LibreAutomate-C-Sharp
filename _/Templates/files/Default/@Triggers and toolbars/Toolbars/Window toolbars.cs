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

//Add here your toolbars that will be attached to windows. Add their triggers in the above function.
//To create new toolbar can be used snippet. Start typing "tool" and select toolbarSnippet in the completion list.
//Click the Run button to apply changes after editing. Then open or activate the trigger window to show the toolbar.
//If the trigger is correct, new toolbar(s) should be in its top-left corner. Shift+drag to some other place. Try to move
//the window to see how it works. You can right-click a toolbar and change its properties.

//Add buttons in the toolbar function. See examples below.
//Button syntax: t["Name|Tooltip"] = o => { code; };
//Quick ways to add a button:
//	Clone an existing button.
//	Snippet: start typing "tool" and select toolbarButtonSnippet in the completion list.
//	To run a script, drag and drop it here from the Files panel. Then in the popup menu select "t[name] = ...".
//	To run/open a file or folder, drag and drop it here from a folder window. Then in the popup menu select "t[name] = ...".
//	To open a web page, drag and drop a link here from a web browser. Then in the popup menu select "t[name] = ...".


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
