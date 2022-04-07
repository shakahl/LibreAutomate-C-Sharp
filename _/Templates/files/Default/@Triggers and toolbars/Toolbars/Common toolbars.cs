using Au.Triggers;

partial class Program {
[Toolbars]
void CommonToolbars() {
	
	//Call toolbar functions here, like in examples.
	
	if (!true) { //examples of triggers. To enable, replace (!true) with (true) and run this script.
		Toolbar_Startup1();
		Triggers.Mouse[TMEdge.TopInCenter50] = Toolbar_ScreenEdge_TopCenter;
	}
}

//Add here your toolbars that will not be attached to windows. They can be attached to screen edges.
//In the above function call toolbar functions, either directlty (to show at startup) or from a trigger.
//To create new toolbar can be used snippet. Start typing "tool" and select toolbarSnippet in the completion list.
//Click the Run button to apply changes after editing.

//Add buttons in the toolbar function. See examples below.
//Button syntax: t["Name|Tooltip"] = o => { code; };
//Quick ways to add a button:
//	Clone an existing button.
//	Snippet: start typing "tool" and select toolbarButtonSnippet in the completion list.
//	To run a script, drag and drop it here from the Files panel. Then in the popup menu select "t[name] = ...".
//	To run/open a file or folder, drag and drop it here from a folder window. Then in the popup menu select "t[name] = ...".
//	To open a web page, drag and drop a link here from a web browser. Then in the popup menu select "t[name] = ...".


#region toolbar examples

void Toolbar_Startup1() {
	var t = new toolbar("Toolbar_Startup1");
	
	//settings
	if (t.FirstTime) {
		
	}
	t.BorderColor = System.Drawing.Color.BlueViolet;
	
	//buttons
	t["A"] = o => {  };
	t["B"] = o => {  };
	t.Menu("C", m => { //drop-down menu
		m["X"] = o => {  };
		m["Y"] = o => {  };
	});
	t.Group("Examples"); //horizontal separator, optionally with text
	t.DisplayText = false;
	t["Run program"] = o => run.it(folders.System + @"notepad.exe");
	t["Script\0 Run script"] = o => script.run("Script example1.cs");
	t["Copy-paste"] = o => {
		string s = clipboard.copy(); //note: to test it, at first select some text somewhere, else it will fail
		s = s.Upper();
		clipboard.paste(s);
	};
//	t["Close", ""] = o => { t.Close(); };
	
	t.Show();
}

void Toolbar_ScreenEdge_TopCenter(MouseTriggerArgs ta) {
	var t = new toolbar("Toolbar_ScreenEdge_TopCenter");
	
	t[""] = o => {  };
	t[""] = o => {  };
	t.Separator();
	t[""] = o => {  };
	t[""] = o => {  };
	t.Group();
	t.Menu("", m => {
		m[""] = o => {  };
		m[""] = o => {  };
	});
	t[""] = o => {  };
	t[""] = o => {  };
	
	//auto-hide at the screen edge of the mouse trigger. Above is the auto-hide part. Below is the always-visible part.
	t = t.AutoHideScreenEdge(ta, 5, ^5, 2);
	t.BorderColor = System.Drawing.Color.Orange;
	
	t.Show();
	ta.DisableTriggerUntilClosed(t); //single instance
}

#endregion

}
