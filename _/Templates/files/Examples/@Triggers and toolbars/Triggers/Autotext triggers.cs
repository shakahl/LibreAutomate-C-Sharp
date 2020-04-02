using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {
[Triggers]
void AutotextTriggers() {
	var tt = Triggers.Autotext;
	var tr = Triggers.Autotext.SimpleReplace;
	
	//examples of autotext triggers
	
	tt["los"] = o => o.Replace("Los Angeles");
	tt["WIndows", TAFlags.MatchCase] = o => o.Replace("Windows");
	
	tt.DefaultPostfixType = TAPostfix.None; //set some options for triggers added afterwards
	tt["<b>"] = o => o.Replace("<b>[[|]]</b>");
	
	Triggers.Options.BeforeAction = o => { AOpt.Key.TextOption = KTextOption.Paste; }; //set AOpt options for trigger actions added afterwards
	tt["#file"] = o => {
		o.Replace("");
		using var fd = new System.Windows.Forms.OpenFileDialog();
		if(fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
		AKeys.Text(fd.FileName);
	};
	
	Triggers.Options.BeforeAction = null; tt.DefaultPostfixType = default; //reset some options
	
	//examples of simple text replacements

	tr["#su"] = "Sunday"; //the same as tt["#su"] = o => o.Replace("Sunday");
	tr["#mo"] = "Monday";
	
	//these triggers will work only in Notepad window
	
	Triggers.Of.Window("* Notepad", "Notepad");
	tr["#tu"] = "Tuesday";
	tr["#we"] = "Wednesday";
	Triggers.Of.AllWindows(); //reset
	
	//with confirmation
	
	tt.DefaultFlags |= TAFlags.Confirm; //add flag
	tr["#th"] = "Thursday";
	tr["#fr", postfixType: TAPostfix.None] = "Friday";
	tt.DefaultFlags &= ~TAFlags.Confirm; //remove flag
	
	//To add triggers can be used snippets. Start typing "trig" and you will see snippets in the completion list.
}


}
