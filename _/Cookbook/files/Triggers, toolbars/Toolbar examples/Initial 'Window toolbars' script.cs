/// The "Window toolbars" script initially is like this:

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

//Add here toolbars that will be attached to windows.
//To create new toolbar can be used snippet toolbarSnippet.
//More info in Cookbook.


#region toolbar examples

void Toolbar_Notepad(WindowTriggerArgs ta = null) {
	var t = new toolbar("Toolbar_Notepad");
	if (t.FirstTime) {
		//Here you can set some initial properties of the toolbar. Later users can change them through the right-click menu.
	}
	
	t["A"] = o => {  };
	t["B|Tooltip"] = o => {  };
	t["|Tooltip", image: "*Modern.TreeLeaf #73BF00"] = o => {  };
	t.Menu("C", m => { //drop-down menu
		m["X"] = o => {  };
		m["Y"] = o => {  };
	});
	t.Separator();
	t["Script"] = o => script.run("Script example1.cs");
	
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
