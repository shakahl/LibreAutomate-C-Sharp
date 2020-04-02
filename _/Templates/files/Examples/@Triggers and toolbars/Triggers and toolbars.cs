/*/ runMode blue; ifRunning warn_restart; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic;
using System.Reflection;
partial class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;

//Don't need to edit this file. Don't delete. Edit/add/delete other files in this folder.

//This code calls functions of this class with [Triggers] or [Toolbars] attribute. They must not have parameters.
//Let the [Triggers] functions add triggers. Recommended: use one function for each trigger type, like in examples.
//Let the [Toolbars] functions create toolbars or add triggers that will create toolbars later.
foreach(var mi in typeof(Script).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance)) {
	if(mi.IsDefined(typeof(TriggersAttribute), false)) _SetTriggerOptions(toolbars: false);
	else if(mi.IsDefined(typeof(ToolbarsAttribute), false)) _SetTriggerOptions(toolbars: true);
	else continue;
	mi.Invoke(this, null);
}

Triggers.Run();

}

/// <summary>
/// Resets trigger options that are common to all trigger types.
/// </summary>
void _SetTriggerOptions(bool toolbars) {
	Triggers.Of.AllWindows();
	Triggers.FuncOf.FollowingTriggers = null;
	Triggers.FuncOf.FollowingTriggersBeforeWindow = null;
	Triggers.FuncOf.NextTrigger = null;
	Triggers.FuncOf.NextTriggerBeforeWindow = null;
	Triggers.Options.BeforeAction = null;
	Triggers.Options.AfterAction = null;
	if(toolbars) Triggers.Options.RunActionInMainThread(); else Triggers.Options.RunActionInThread(0);
}
}

[AttributeUsage(AttributeTargets.Method)] class TriggersAttribute : Attribute {  }
[AttributeUsage(AttributeTargets.Method)] class ToolbarsAttribute : Attribute {  }
