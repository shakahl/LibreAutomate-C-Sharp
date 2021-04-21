/*/ ifRunning warn_restart; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using Au.Triggers;
using System.Reflection;
partial class Script { [STAThread] static void Main(string[] a) { ATask.Setup(trayIcon: false); new Script(a); } Script(string[] args) { //;;;

//Here you can add code that runs at startup. Set variables, etc.
//Add triggers and toolbars in other files of this project. More info in "Readme.txt".



RunTriggersAndToolbars();
}

//set these fields = true or false to enable or disable the example triggers and/or toolbars
bool _enableHotkeyTriggerExamples = true;
bool _enableAutotextTriggerExamples = true;
bool _enableMouseTriggerExamples = false;
bool _enableWindowTriggerExamples = true;
bool _enableToolbarExamples = true;

}

//. RunTriggersAndToolbars

//This code manages your triggers and toolbars. Don't delete, it is not an example code.

partial class Script {

/// <summary>
/// Use this to add triggers, set trigger options, etc. Example:<br/> <c>Triggers.Hotkey["Ctrl+E"] = o => AOutput.Write(o.Trigger);</c>.
/// </summary>
ActionTriggers Triggers { get; } = new();

/// <summary>
/// Calls your functions that have attribute [Triggers] or [Toolbars]. Finally calls <c>Triggers.Run();</c>.
/// Let [Triggers] functions add triggers. Let [Toolbars] functions create some toolbars and/or add triggers for other toolbars.
/// </summary>
/// <remarks>
/// Before calling each function, resets all trigger options (Options, Of, FuncOf, etc), to avoid inheriting options changed in other functions.
/// </remarks>
void RunTriggersAndToolbars() {
	void _TriggerOptions(bool toolbars) {
		//Here you can set options that are common to all your triggers. Examples:
//		if (!toolbars) {
//			Triggers.Options.Thread(0, ifRunningWaitMS: 500);
//			Triggers.Options.BeforeAction = o => {
//				AOpt.Key.SleepFinally = 50;
//			};
//		}
	}
	
	foreach(var mi in typeof(Script).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance)) {
		bool tb = mi.IsDefined(typeof(ToolbarsAttribute), false);
		if(!(tb || mi.IsDefined(typeof(TriggersAttribute), false))) continue;
		
		Triggers.ResetOptions();
		_TriggerOptions(toolbars: tb);
		if(tb) Triggers.Options.ThreadMain();
		
		try { mi.Invoke(this, null); }
		catch(TargetInvocationException ex) { AOutput.Write(ex.InnerException); return; }
	}
	
	Triggers.Run();
}

[AttributeUsage(AttributeTargets.Method)] class TriggersAttribute : Attribute {  }
[AttributeUsage(AttributeTargets.Method)] class ToolbarsAttribute : Attribute {  }

}

//;
