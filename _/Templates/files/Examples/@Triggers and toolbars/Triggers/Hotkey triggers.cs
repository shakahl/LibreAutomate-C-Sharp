using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {
[Triggers]
void HotkeyTriggers() {
	var hk = Triggers.Hotkey;
	
	//examples of hotkey triggers
	
	hk["Ctrl+Alt+K"] = o => AOutput.Write(o.Trigger); //it means: when I press Ctrl+Alt+K, execute action "AOutput.Write(o.Trigger)"
	hk["Ctrl+Shift+F11"] = o => { //action can have multiple statements
		var w1 = AWnd.FindOrRun("* Notepad", run: () => AExec.Run(AFolders.System + "notepad.exe"));
		AKeys.Text("text");
		500.ms();
		w1.Close();
	};
	hk["Ctrl+Shift+1"] = o => TriggerActionExample(); //action code can be in other function. To find it quickly, Ctrl+click the function name here.
	hk["Ctrl+Shift+2"] = o => TriggerActionExample2(o.Window); //the function can be in any class or partial file of this project folder
	hk["Ctrl+Shift+3"] = o => ATask.Run("Script example1.cs"); //run script in separate process. Then don't need to restart triggers when editing the script.

	//triggers that work only with some windows (when the window is active)

	Triggers.Of.Window("* WordPad", "WordPadClass");
	
	hk["Ctrl+F5"] = o => AOutput.Write("action 1", o.Trigger, o.Window);
	//hk[""] = o => {  };
	//...

	Triggers.Of.Windows(",,notepad.exe"); //all windows of notepad.exe process
	
	hk["Ctrl+F5"] = o => AOutput.Write("action 2", o.Trigger, o.Window);
	//hk[""] = o => {  };
	//...

	//...
	
	//disable/enable triggers
	Triggers.Of.AllWindows();
	hk["Ctrl+Alt+Win+D"] = o => ActionTriggers.DisabledEverywhere ^= true;
	hk.Last.EnabledAlways = true;
	
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
	//For more info click word ActionTriggers above and press F1.
}


void TriggerActionExample() {
	AOutput.Write("TriggerActionExample");
}

}
