//This file contains code that makes triggers and toolbars alive. Don't delete.
//Don't need to edit or understand, unless you want to set some trigger options here.

using Au; using Au.Types; using System; using System.Collections.Generic; using System.Reflection;

partial class Script {
/// <summary>
/// Calls functions that have attribute [Triggers] or [Toolbars].
/// Let [Triggers] functions add triggers.
/// Let [Toolbars] functions create some toolbars and/or add triggers for other toolbars.
/// Finally calls Triggers.Run; it waits for trigger events etc.
/// </summary>
/// <remarks>
/// Before calling each function resets all trigger options (including FuncOf etc), therefore they don't inherit options changed in other functions.
/// Also here you can set some common options for triggers defined in these functions. Set other options in these functions, not here.
/// </remarks>
void RunTriggersAndToolbars() {
	void _TriggerOptions(bool toolbars) {
		//Examples:
//		if (!toolbars) {
//			Triggers.Options.RunActionInThread(0, ifRunningWaitMS: 500);
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
		if(tb) Triggers.Options.RunActionInMainThread();
		mi.Invoke(this, null);
	}
	
	Triggers.Run();
}

[AttributeUsage(AttributeTargets.Method)] class TriggersAttribute : Attribute {  }
[AttributeUsage(AttributeTargets.Method)] class ToolbarsAttribute : Attribute {  }
}
