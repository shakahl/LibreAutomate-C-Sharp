/// The "Common toolbars" script initially is like this:

using Au.Triggers;

partial class Program {
[Toolbars]
void CommonToolbars() {
	
	//Call toolbar functions here, like in examples.
	
	if (!true) { //examples of triggers. To enable, replace (!true) with (true) and run this script.
		Toolbar_Startup1(); //call directly
		Triggers.Mouse[TMEdge.TopInCenter50] = Toolbar_ScreenEdge_TopCenter; //or use a trigger
	}
}

//Add here toolbars that will not be attached to windows. They can be attached to screen edges.
//To create new toolbar can be used snippet toolbarSnippet.
//More info in Cookbook.


#region toolbar examples

void Toolbar_Startup1() {
	var t = new toolbar("Toolbar_Startup1");
	
	//settings
	if (t.FirstTime) {
		//Here you can set some initial properties of the toolbar. Later users can change them through the right-click menu.
	}
	t.BorderColor = System.Drawing.Color.BlueViolet;
	
	//buttons
	t["A"] = o => {  };
	t["B|Tooltip"] = o => {  };
	t["|Tooltip", image: "*WeatherIcons.RainMix #2F33D0"] = o => {  };
	t.Menu("C", m => { //drop-down menu
		m["X"] = o => {  };
		m["Y"] = o => {  };
	});
	t.Group("Examples"); //horizontal separator, optionally with text
	t.DisplayText = false;
	t["Run program"] = o => run.it(folders.System + @"notepad.exe");
	t["Script"] = o => script.run("Script example1.cs");
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
