using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using Au.Triggers;

partial class Script {

[Toolbars]
void CommonToolbars() {
	
	//Call toolbar functions here, like in examples.
	
	if (_enableToolbarExamples) {
		//examples
		
		Toolbar_Startup1();
		Triggers.Mouse[TMEdge.TopInCenter50] = Toolbar_ScreenEdge_TopCenter;
	}
}

//Add here your toolbars that are common to all windows.
//In the above function call toolbar functions, either directlty (to show at startup) or from a trigger.
//To create toolbars and add buttons can be used snippets. Start typing "tool" and you will see snippets in the completion list.



#region toolbar examples

void Toolbar_Startup1() {
	var t = new AToolbar("Toolbar_Startup1");
	
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
	t["Run program"] = o => AFile.Run(AFolders.System + @"notepad.exe");
	t["Script\0 Run script"] = o => ATask.Run("Script example1.cs");
	t["Copy-paste"] = o => {
		string s = AClipboard.Copy(); //note: to test it, at first select some text somewhere, else it will fail
		s = s.Upper();
		AClipboard.Paste(s);
	};
//	t["Close", ""] = o => { t.Close(); };
	
	t.Show();
}

void Toolbar_ScreenEdge_TopCenter(MouseTriggerArgs ta) {
	var t = new AToolbar("Toolbar_ScreenEdge_TopCenter");
	
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
	t = t.AutoHideScreenEdge(ta, 5, Coord.Reverse(5), 2);
	t.BorderColor = System.Drawing.Color.Orange;
	
	t.Show();
	ta.DisableTriggerUntilClosed(t); //single instance
}

#endregion

}
