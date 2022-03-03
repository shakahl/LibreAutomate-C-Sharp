/// To quickly insert <see cref="wpfBuilder"/> code, use snippet wpfSnippet: type wpf and select from the list. Also look in menu File -> New -> Dialogs.

using System.Windows;
using System.Windows.Controls;

/// Add controls, show dialog, get final control values.

var b = new wpfBuilder("Window").WinSize(400);
b.R.Add("Text", out TextBox text1).Focus();
b.R.Add("Combo", out ComboBox combo1).Items("Zero|One|Two");
b.R.Add(out CheckBox check1, "Check");
b.R.AddButton("Button", _ => { print.it("Button clicked"); });
b.R.AddOkCancel();
b.End();
if (!b.ShowDialog()) return;
print.it(text1.Text, combo1.SelectedIndex, check1.IsChecked == true);

/// Field value validation.

var b2 = new wpfBuilder("Window").WinSize(400);
b2.R.Add("Count", out TextBox text2).Focus().Validation(e => { return int.TryParse((e as TextBox).Text, out int i) && i >= 0 && i <= 100 ? null : "Count must be an integer number 0-100";  });
b2.R.AddOkCancel();
b2.End();
if (!b2.ShowDialog()) return;
int count = int.Parse(text2.Text);
print.it(count);

/// Custom columns and rows.

var b3 = new wpfBuilder("Window").WinSize(400, 200);
b3.Columns(0, 100, -1); //3 columns. Widths: first - auto, second - 100, third - what remains.
b3.R.Add("Text", out TextBox _).Add(out CheckBox _, "Check 1");
b3.R.Add("Longer text", out TextBox _).Add(out CheckBox _, "Check 2");
b3.Row(-1).Add(out TextBox _).Span(2).Multiline(); //this row will be resized vertically when resizing the window. Window height must be specified.
b3.R.AddOkCancel();
b3.End();
if (!b3.ShowDialog()) return;

/// Nested panels and group box.

var b4 = new wpfBuilder("Window").WinSize(400);
b4.R.Add("Text", out TextBox _).Focus();
b4.R.StartStack();
for (int i = 0; i < 5; i++) { b4.Add(out CheckBox _, "Check " + i); }
b4.End();
b4.R.StartGrid<GroupBox>("Group");
b4.Columns(0, -1, 100);
b4.R.Add("Text 1", out TextBox _).Add(out CheckBox _, "Check");
b4.R.Add("Text 2", out TextBox _).Span(1);
b4.End();
b4.R.AddOkCancel();
b4.End();
if (!b4.ShowDialog()) return;

/// Select date.

var b5 = new wpfBuilder("Window").WinSize(400);
b5.R.Add("Text", out DatePicker d1);
d1.SelectedDate = DateTime.Now;
d1.SelectedDateFormat = DatePickerFormat.Long;
b5.R.AddOkCancel();
b5.End();
if (!b5.ShowDialog()) return;
var date = d1.SelectedDate ?? DateTime.Now.Date;
print.it(date);

/// Events.

var b6 = new wpfBuilder("Window").WinSize(400);
b6.R.Add("Text", out TextBox t1).Focus();
b6.R.Add("List", out ListBox list1).Items("Zero|One|Two");
b6.R.Add(out CheckBox c1, "Check");
b6.R.AddButton("Button", _ => { print.it("Button clicked"); c1.IsChecked = true; });
b6.R.AddOkCancel();
b6.End();

t1.TextChanged += (o, e) => {
	var v = o as TextBox; //o is t1, but assume you don't want to use the t1 variable here
	print.it("text changed", v.Text);
};

list1.SelectionChanged += (o, e) => {
	print.it("selected", list1.SelectedIndex, list1.SelectedItem);
};

c1.Click += (o, e) => {
	print.it("checkbox clicked", c1.IsChecked);
};

c1.Checked += _CheckedUnchecked;
c1.Unchecked += _CheckedUnchecked;

void _CheckedUnchecked(object o, RoutedEventArgs e) {
	print.it("checked/unchecked", c1.IsChecked);
}

if (!b6.ShowDialog()) return;
