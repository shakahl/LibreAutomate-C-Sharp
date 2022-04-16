using Au.Triggers;

partial class Program {
[Triggers]
void HotkeyTriggers() {
	var hk = Triggers.Hotkey;
	
	//Add hotkey triggers here.
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
	//Click the Run button to apply changes after editing.
	
	
	
	if (!true) { //examples. To enable, replace (!true) with (true) and run this script.
		hk["Ctrl+Alt+K"] = o => print.it("trigger action example 1", o.Trigger); //it means: when I press Ctrl+Alt+K, execute trigger action print.it(...)
		hk["Ctrl+Shift+F11"] = o => { //multiple statements
			var w1 = wnd.active;
			if(w1.Name.Like("* - Notepad")) keys.sendt("trigger action example 2\r\n");
			else print.it("trigger action example 2");
		};
		hk["Ctrl+Shift+1"] = o => TriggerActionExample(); //call other function. To find it quickly, Ctrl+click the function name here.
		hk["Ctrl+Shift+2"] = o => TriggerActionExample2(o.Window); //the function can be in any class or partial file of this project
		hk["Ctrl+Shift+3"] = o => script.run("Script example1.cs"); //run script in separate process
		
		//triggers that work only with some windows (when the window is active)
		
		Triggers.Of.Window("* WordPad", "WordPadClass");
		
		hk["Ctrl+F5"] = o => print.it("trigger action example 5", o.Trigger, o.Window);
		//hk[""] = o => {  };
		//...

		Triggers.Of.Window(of: "notepad.exe"); //all windows of notepad.exe process
		
		hk["Ctrl+F5"] = o => print.it("trigger action example 6", o.Trigger, o.Window);
		//hk[""] = o => {  };
		//...
		
		//...
		
		//disable/enable triggers
		Triggers.Of.AllWindows();
		hk["Ctrl+Alt+Win+D"] = o => ActionTriggers.DisabledEverywhere ^= true;
		hk.Last.EnabledAlways = true;
	}
}


void TriggerActionExample() {
	print.it("trigger action example 3");
}

}
