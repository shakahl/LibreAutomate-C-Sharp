using Au; using Au.Types; using System; using System.Collections.Generic;

partial class Script {
[Toolbars]
void CommonToolbars() {
	_Toolbar_Common1();
	//_Toolbar_Common2();
	//...
}

void _Toolbar_Common1() {
	var t = new AToolbar("_Toolbar_Common1");
	if(!t.SettingsModified) {
		//first time set initial properties that are in the right-click menu. Later use saved values.
		t.AutoSize = true;
	}
	//set other properties
	t.BorderColor = System.Drawing.Color.Olive;
	
	//add buttons
	t["A"] = o => {  };
	t["B"] = o => {  };
	t.MenuButton("C", m => { //drop-down menu
		m["X"] = o => {  };
		m["Y"] = o => {  };
	});
	t.Group("Examples"); //horizontal separator, optionally with text
	t.NoText = true;
	t["Run program"] = o => AExec.Run(AFolders.System + @"notepad.exe");
	t["Run script"] = o => ATask.Run("Script example1.cs");
	t["Copy-paste"] = o => {
		string s = AClipboard.Copy(); //note: to test it, at first select some text somewhere, else it will fail
		s = s.Upper();
		AClipboard.Paste(s);
	};
	
	bool autoHide = false; //or true
	if(autoHide) {
		//An "auto-hide" toolbar actually consists of 2 toolbars:
		//	t - toolbar with many buttons. Hidden when mouse pointer isn't in toolbar t2.
		//	t2 - small toolbar with zero or few buttons. It is the owner of toolbar t.
		var t2 = new AToolbar(t.Name + "^") { Satellite = t };
		t2.Show();
	} else {
		t.Show();
	}
}

//void _Toolbar_Common2() { ... }

//...

//To create toolbars and add buttons can be used snippets. Start typing "toolbar" and you will see snippets in the completion list.

}
