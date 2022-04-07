//This file can be used as a template for new files containing window toolbars. To create new file:
//1. Clone this file (Ctrl+drag) to the same folder.
//2. Open the cloned file. Rename.
//3. Rename function RenameMeToolbars. It must be a unique name in this project folder. For example CalculatorToolbars.
//4. Add and edit one or more toolbars. You can add/delete/edit everything later too.

//To add a toolbar you can use snippet toolbarSnippet:
//1. Start typing "tool" and in the completion list select "toolbarSnippet".
//2. In the added code replace all Toolbar_RenameMe with a unique name, for example Toolbar_X.
//3. In the renamed RenameMeToolbars function add a window trigger, like in examples. Edit window name and other arguments.
//4. Edit the toolbar code. Add/delete/edit buttons. Set properties if need.

using Au.Triggers;

partial class Program {
[Toolbars]
void RenameMeToolbars() {
	
	//Add toolbar triggers here, like in examples.
	
	//Triggers.Window[TWEvent.ActiveOnce, "*Notepad", "Notepad"] = Toolbar_RenameMe;
	//Triggers.Window[TWEvent.ActiveOnce, "*Name1", "ClassName1"] = Toolbar_OtherWindow;
	//Triggers.Window[TWEvent.ActiveOnce, "*Name2", "ClassName2"] = o => { Toolbar_One(o); Toolbar_Two(o); } //attach 2 toolbars to this window
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


void Toolbar_RenameMe(WindowTriggerArgs ta = null) {
	var t = new toolbar("Toolbar_RenameMe");
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

}
