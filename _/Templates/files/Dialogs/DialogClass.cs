using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/*
This class file can be used in scripts etc like any class file. One of:
- Add this file to the same project folder.
- Or in script Properties dialog click button "Class file", select this file, OK.
- Or add this file to a library project and use the library anywhere.
*/

namespace Dialogs {
/// <summary>
/// 
/// </summary>
/// <example>
/// <code><![CDATA[
/// var d = new Dialogs.DialogClass();
/// d.ShowDialog();
/// ]]></code>
/// </example>
public class DialogClass : Window {
	///
	public DialogClass() {
		Title = "Dialog";
		var b = new AWpfBuilder(this).WinSize(400);
		b.R.Add("Text", out TextBox text1).Focus().Validation(_ => string.IsNullOrWhiteSpace(text1.Text) ? "Text cannot be empty" : null);
		b.R.Add("Combo", out ComboBox combo1).Items("Zero|One|Two");
		b.R.Add(out CheckBox c1, "Check");
		b.R.AddOkCancel();
		b.End();
		
		//if need, set initial control values here or in Loaded event handler below
		
//		bool loaded = false;
//		b.Loaded += ()=> {
//			loaded = true;
//		};

		b.OkApply += e => {
//			if (!loaded) return;
			
			AOutput.Write($"Text: \"{text1.Text.Trim()}\"");
			AOutput.Write($"Combo index: {combo1.SelectedIndex}");
			AOutput.Write($"Check: {c1.IsCheck()}");
		};
	}
}
}

//See also: snippet wpfSnippet.
