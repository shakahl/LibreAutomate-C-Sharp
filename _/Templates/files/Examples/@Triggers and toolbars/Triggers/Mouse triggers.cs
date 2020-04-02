using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {
[Triggers]
void MouseTriggers() {
	//examples of mouse triggers

	Triggers.Mouse[TMClick.Right, "Ctrl+Shift", TMFlags.ButtonModUp] = o => AOutput.Write(o.Trigger); //Ctrl+Shift+RightClick
	Triggers.Mouse[TMEdge.RightInCenter50] = o => { //the right edge of the primary screen, center 50%
		var m = new AMenu("example");
		m["A"] = o => {  };
		m["B"] = o => {  };
		using(m.Submenu("C")) {
			m["D"] = o => {  };
			m["E"] = o => {  };
		}
		m.Show();
		
		//To create menus can be used snippet menuSnippet. Start typing "menu" and you will see snippets in the completion list.
	};
	Triggers.Mouse[TMMove.LeftRightInCenter50, screen: TMScreen.Any] = o => AWnd.SwitchActiveWindow(); //move the mouse quickly to the left and back in center 50% of any screen
	Triggers.Mouse[TMMove.RightLeftInCenter50, screen: TMScreen.Any] = o => AKeys.Key("Ctrl+Tab"); //to the right and back. Ctrl+Tab should switch the active document.

	Triggers.FuncOf.NextTrigger = o => AKeys.IsScrollLock; //example of a custom scope (aka context, condition)
	Triggers.Mouse[TMWheel.Forward] = o => AOutput.Write($"{o.Trigger} while ScrollLock is on");
	
}


}
