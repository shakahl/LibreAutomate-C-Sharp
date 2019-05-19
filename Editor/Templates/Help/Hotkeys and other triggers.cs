/*/ runMode blue; ifRunning restart; /*/ //{{
//{{ using
using Au; using static Au.AStatic; using Au.Types; using System; using System.Collections.Generic; //}}
using Au.Triggers; //need this for triggers
using System.Windows.Forms; //need this for some examples in this script

//{{ main
unsafe partial class Script :AScript { [STAThread] static void Main(string[] args) { new Script()._Main(args); } void _Main(string[] args) { //}}//}}//}}//}}

//Triggers are used to execute parts of a running script. Not to launch scripts.
//This script contains examples of all trigger types. The code is copied from the ActionTriggers class help topic.
//To test this script, click the Run button on the toolbar. You can edit it and then click Run to restart.
//To run a script when this program starts and loads this workspace, add its name in Options -> General -> Run scripts...
//Bor best performance, all your triggers should be in a single script. Its role should be miniProgram (default). Multiple scripts and exeProgram scripts together use more CPU.
//	But it's OK if several such scripts sometimes run simultaneously.
//		Example: when you don't want to edit/restart your main script when testing new triggers and their actions.
//		Example: when testing triggers in .exe scripts (role exeProgram).


//if you want to set options for all or some triggers, do it before adding them
Triggers.Options.RunActionInThread(0, 500);

//you can use variables if don't want to type "Triggers.Hotkey" etc for each trigger
var hk = Triggers.Hotkey;
var mouse = Triggers.Mouse;
var window = Triggers.Window;
var tt = Triggers.Autotext;

//hotkey triggers

hk["Ctrl+K"] = o => Print(o.Trigger); //it means: execute code "o => Print(o.Trigger)" when I press Ctrl+K
hk["Ctrl+Shift+F11"] = o => {
	Print(o.Trigger);
	var w1 = AWnd.FindOrRun("* Notepad", run: () => AExec.Run(AFolders.System + "notepad.exe"));
	Text("text");
	w1.Close();
};

//triggers that work only with some windows

Triggers.Of.Window("* WordPad", "WordPadClass"); //let the following triggers work only when a WordPad window is active
hk["Ctrl+F5"] = o => Print(o.Trigger, o.Window);
hk["Ctrl+F6"] = o => Print(o.Trigger, o.Window);

var notepad = Triggers.Of.Window("* Notepad"); //let the following triggers work only when a Notepad window is active
hk["Ctrl+F5"] = o => Print(o.Trigger, o.Window);
hk["Ctrl+F6"] = o => Print(o.Trigger, o.Window);

Triggers.Of.AllWindows(); //let the following triggers work with all windows

//mouse triggers

mouse[TMClick.Right, "Ctrl+Shift", TMFlags.ButtonModUp] = o => Print(o.Trigger);
mouse[TMEdge.RightInCenter50] = o => { Print(o.Trigger); ADialog.ShowEx("Bang!", x: Coord.Max); };
mouse[TMMove.LeftRightInCenter50] = o => AWnd.SwitchActiveWindow();

Triggers.FuncOf.NextTrigger = o => AKeyboard.IsScrollLock; //example of a custom scope (aka context, condition)
mouse[TMWheel.Forward] = o => Print($"{o.Trigger} while ScrollLock is on");

Triggers.Of.Again(notepad); //let the following triggers work only when a Notepad window is active
mouse[TMMove.LeftRightInBottom25] = o => { Print(o.Trigger); o.Window.Close(); };
Triggers.Of.AllWindows();

//window triggers. Note: window triggers don't depend on Triggers.Of.

window[TWEvent.ActiveNew, "* Notepad", "Notepad"] = o => Print("opened Notepad window");
window[TWEvent.ActiveNew, "Notepad", "#32770", contains: "Do you want to save *"] = o => {
	Print("opened Notepad's 'Do you want to save' dialog");
	//Key("Alt+S"); //press button Save
};

//autotext triggers

tt["los"] = o => o.Replace("Los Angeles");
tt["WIndows", TAFlags.MatchCase] = o => o.Replace("Windows");
tt.DefaultPostfixType = TAPostfix.None;
tt["<b>"] = o => o.Replace("<b>[[|]]</b>");
Triggers.Options.BeforeAction = o => { Opt.Key.TextOption = KTextOption.Paste; }; //the best way to set thread-local options
tt["#file"] = o => {
	o.Replace("");
	var fd = new OpenFileDialog();
	if(fd.ShowDialog() == DialogResult.OK) Text(fd.FileName);
};
Triggers.Options.BeforeAction = null;
tt.DefaultPostfixType = default;

var ts = Triggers.Autotext.SimpleReplace;
ts["#su"] = "Sunday"; //the same as tt["#su"] = o => o.Replace("Sunday");
ts["#mo"] = "Monday";

//how to stop and disable/enable triggers

hk["Ctrl+Alt+Q"] = o => Triggers.Stop(); //let Triggers.Run() end its work and return
hk.Last.EnabledAlways = true;

hk["Ctrl+Alt+D"] = o => Triggers.Disabled ^= true; //disable/enable triggers here
hk.Last.EnabledAlways = true;

hk["Ctrl+Alt+Win+D"] = o => ActionTriggers.DisabledEverywhere ^= true; //disable/enable triggers in all processes
hk.Last.EnabledAlways = true;

hk["Ctrl+F7"] = o => Print("This trigger can be disabled/enabled with Ctrl+F8.");
var t1 = hk.Last;
hk["Ctrl+F8"] = o => t1.Disabled ^= true; //disable/enable a trigger

//finally call Triggers.Run(). Without it the triggers won't work.
Triggers.Run();
//Triggers.Run returns when is called Triggers.Stop (see the "Ctrl+Alt+Q" trigger above).
Print("called Triggers.Stop");

//Recommended properties for scripts containg triggers: 'runMode'='blue' and 'ifRunning'='restart'. You can set it in the Properties dialog.
//The first property allows other scripts to start while this script is running.
//The second property makes easy to restart the script after editing: just click the Run button.
