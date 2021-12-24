//.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

script.setup(trayIcon: true);
//..

var bMain = new wpfBuilder("Window").WinSize(400);
bMain.Row(-1).Add(out TabControl tc).Height(300..);
bMain.R.AddOkCancel(apply: "_Apply");

wpfBuilder _Page(string name, WBPanelType panelType = WBPanelType.Grid) {
	var tp = new TabItem { Header = name };
	tc.Items.Add(tp);
	return new wpfBuilder(tp, panelType);
}

//--------------------------------

var b1 = _Page("Page1");
b1.R.Add("Text", out TextBox _);
b1.AddButton("Close 5", 5);
b1.End();

//--------------------------------

var b2 = _Page("Page2");
b2.R.Add("Combo", out ComboBox _).Editable().Items("Zero|One|Two");
b2.R.Add(out CheckBox _, "Check");
b2.End();

//--------------------------------

//tc.SelectedIndex = 1;

bMain.End();
if (!bMain.ShowDialog()) return;
//print.it(b1.ResultButton);
