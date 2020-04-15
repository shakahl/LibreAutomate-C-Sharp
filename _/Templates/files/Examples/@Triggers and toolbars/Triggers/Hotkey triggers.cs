using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {
[Triggers]
void HotkeyTriggers() {
	var hk = Triggers.Hotkey;
	
	//Add hotkey triggers here.
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
	//For more info click word ActionTriggers below and press F1.
	
	
	
	if (_enableHotkeyTriggerExamples) {
		//examples of hotkey triggers
		
		hk["Ctrl+Alt+K"] = o => AOutput.Write("trigger action example 1", o.Trigger); //it means: when I press Ctrl+Alt+K, execute trigger action AOutput.Write(...)
		hk["Ctrl+Shift+F11"] = o => { //multiple statements. To hide the code, click the [-] box at the left.
			var w1 = AWnd.FindOrRun("* Notepad", run: () => AExec.Run(AFolders.System + "notepad.exe"));
			AKeys.Text("trigger action example 2");
			500.ms();
			w1.Close();
		};
		hk["Ctrl+Shift+1"] = o => TriggerActionExample(); //call other function. To find it quickly, Ctrl+click the function name here.
		hk["Ctrl+Shift+2"] = o => TriggerActionExample2(o.Window); //the function can be in any class or partial file of this project
		hk["Ctrl+Shift+3"] = o => ATask.Run("Script example1.cs"); //run script in separate process
		
		//triggers that work only with some windows (when the window is active)
		
		Triggers.Of.Window("* WordPad", "WordPadClass");
		
		hk["Ctrl+F5"] = o => AOutput.Write("trigger action example 5", o.Trigger, o.Window);
		//hk[""] = o => {  };
		//...

		Triggers.Of.Windows(",,notepad.exe"); //all windows of notepad.exe process
		
		hk["Ctrl+F5"] = o => AOutput.Write("trigger action example 6", o.Trigger, o.Window);
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
	AOutput.Write("trigger action example 3");
}

}
