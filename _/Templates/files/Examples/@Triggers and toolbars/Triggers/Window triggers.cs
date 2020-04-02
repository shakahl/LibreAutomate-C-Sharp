using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {
[Triggers]
void WindowTriggers() {
	//examples of window triggers. Note: window triggers don't depend on Triggers.Of.

	Triggers.Window[TWEvent.ActiveNew, "* Notepad", "Notepad"] = o => AOutput.Write("opened Notepad window");
	Triggers.Window[TWEvent.ActiveNew, "Notepad", "#32770", contains: "Do you want to save *"] = o => {
		AOutput.Write("opened Notepad's \"Do you want to save\" dialog");
		//AKeys.Key("Alt+S"); //press button Save
	};
	
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
}


}
