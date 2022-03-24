using System.Windows;
using System.Windows.Controls;

var b = new wpfBuilder("Window").WinSize(400);

b.Row(100).Add(out DataGrid d1);


b.R.AddOkCancel();
b.End();
if (!b.ShowDialog()) return;
