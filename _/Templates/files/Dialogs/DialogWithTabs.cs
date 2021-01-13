using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
/// var d = new Dialogs.DialogWithTabs();
/// d.ShowDialog();
/// ]]></code>
/// </example>
public class DialogWithTabs : Window {
	AWpfBuilder _b;
	TabControl _tc;

	///
	public DialogWithTabs() {
		Title = "Dialog";
		_b = new AWpfBuilder(this).WinSize(400);
		_b.Row(-1).Add(out _tc).Height(300..);
		_b.R.AddOkCancel(apply: "_Apply");
		_Page1();
		_Page2();
		//...
		_b.End();
		//_tc.SelectedIndex = 1;
	}

	AWpfBuilder _Page(string name, WBPanelType panelType = WBPanelType.Grid) {
		var tp = new TabItem { Header = name };
		_tc.Items.Add(tp);
		return new AWpfBuilder(tp, panelType).Margin("3");
	}

	void _Page1() {
		var b = _Page("Page1");
		b.R.Add("Text", out TextBox text1).Validation(_ => string.IsNullOrWhiteSpace(text1.Text) ? "Text cannot be empty" : null);
		b.End();
		
		//if need, set initial control values here or in Loaded event handler below
		
//		bool loaded = false;
//		b.Loaded += ()=> {
//			loaded = true;
//		};

		_b.OkApply += e => {
//			if (!loaded) return;
			
			AOutput.Write($"Text: \"{text1.Text.Trim()}\"");
		};
	}

	void _Page2() {
		var b = _Page("Page2");
		b.R.Add("Combo", out ComboBox combo1).Items("Zero|One|Two");
		b.R.Add(out CheckBox c1, "Check");
		b.End();
	
//		bool loaded = false;
//		b.Loaded += ()=> {
//			loaded = true;
//		};

		_b.OkApply += e => {
//			if (!loaded) return;
			
			AOutput.Write($"Combo index: {combo1.SelectedIndex}");
			AOutput.Write($"Check: {c1.IsCheck()}");
		};
	}
	
	//...
}
}

//See also: snippet wpfSnippet.
