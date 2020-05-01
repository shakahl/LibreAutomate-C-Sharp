using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using Au.Triggers;

partial class Script {
[Triggers]
void AutotextTriggers() {
	var tt = Triggers.Autotext;
	var tr = Triggers.Autotext.SimpleReplace;
	
	//Add autotext triggers here.
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
	
	
	
	if (_enableAutotextTriggerExamples) {
		//examples of autotext triggers
		
		tt["losa"] = o => o.Replace("Los Angeles (autotext example)");
		tt["WIndows", TAFlags.MatchCase] = o => o.Replace("Windows (autotext example)");
		
		tt.DefaultPostfixType = TAPostfix.None; //set some options for triggers added afterwards
		tt["<b>"] = o => o.Replace("<b>[[|]]</b>");
		
		Triggers.Options.BeforeAction = o => { AOpt.Key.TextOption = KTextOption.Paste; }; //set AOpt options for trigger actions added afterwards
		tt["#file"] = o => {
			o.Replace("");
			using var fd = new System.Windows.Forms.OpenFileDialog { Title = "Autotext example" };
			if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			AKeys.Text(fd.FileName);
		};
		
		Triggers.Options.BeforeAction = null; tt.DefaultPostfixType = default; //reset some options
		
		//examples of simple text replacements
		
		tr["#su"] = "Sunday (autotext example)"; //the same as tt["#su"] = o => o.Replace("Sunday");
		tr["#mo"] = "Monday (autotext example)";
		
		//these triggers will work only in Notepad window
		
		Triggers.Of.Window("* Notepad", "Notepad");
		tr["#tu"] = "Tuesday (autotext example)";
		tr["#we"] = "Wednesday (autotext example)";
		Triggers.Of.AllWindows(); //reset
		
		//with confirmation
		
		tt.DefaultFlags |= TAFlags.Confirm; //add flag
		tr["#th"] = "Thursday (autotext example)";
		tr["#fr", postfixType: TAPostfix.None] = "Friday (autotext example)";
		tt.DefaultFlags &= ~TAFlags.Confirm; //remove flag
	}
}


}
