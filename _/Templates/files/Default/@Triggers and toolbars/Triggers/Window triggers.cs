using Au.Triggers;

partial class Script {
[Triggers]
void WindowTriggers() {
	Triggers.Options.ThreadNew();
	
	//Add window triggers here.
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
	
	
	
	if (_enableWindowTriggerExamples) {
		//examples of window triggers
		
		Triggers.Window[TWEvent.ActiveNew, "* Notepad", "Notepad"] = o => print.it("window trigger example 1");
		Triggers.Window[TWEvent.ActiveNew, "Notepad", "#32770", contains: "Do you want to save *"] = o => {
			print.it("window trigger example 2");
			//keys.send("Alt+S"); //press button Save
		};
		
		//note: window triggers don't depend on Triggers.Of.
	}
}


}
