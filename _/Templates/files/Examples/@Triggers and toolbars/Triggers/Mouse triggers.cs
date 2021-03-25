using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using Au.Triggers;

partial class Script {
[Triggers]
void MouseTriggers() {
	
	//Add mouse triggers here.
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
	
	
	
	if (_enableMouseTriggerExamples) {
		//examples of mouse triggers. Initially disabled. To enable, in the main script file set _enableMouseTriggerExamples = true;
		
		Triggers.Mouse[TMClick.Right, "Ctrl+Shift", TMFlags.ButtonModUp] = o => AOutput.Write("mouse trigger example", o.Trigger); //Ctrl+Shift+RightClick
		Triggers.Mouse[TMEdge.RightInCenter50] = o => { //the right edge of the primary screen, center 50%
			var m = new AMenu("example");
			m["A (mouse trigger example)"] = o => {  };
			m["B"] = o => {  };
			m.Submenu("C", m => {
				m["D"] = o => {  };
				m["E"] = o => {  };
			});
			m.Show();
			
			//To create menus can be used snippets. Start typing "menu" and you will see snippets in the completion list.
		};
		Triggers.Mouse[TMMove.LeftRightInCenter50, screen: TMScreen.Any] = o => AWnd.SwitchActiveWindow(); //move the mouse quickly to the left and back in center 50% of any screen
		Triggers.Mouse[TMMove.RightLeftInCenter50, screen: TMScreen.Any] = o => AKeys.Key("Ctrl+Tab"); //to the right and back. Ctrl+Tab should switch the active document.
		
		Triggers.FuncOf.NextTrigger = o => AKeys.IsScrollLock; //example of a custom scope (aka context, condition)
		Triggers.Mouse[TMWheel.Forward] = o => AOutput.Write($"mouse trigger example: {o.Trigger} while ScrollLock is on");
	}
}


}
