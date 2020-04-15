/*/ runMode blue; ifRunning warn_restart; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic;
partial class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;

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
