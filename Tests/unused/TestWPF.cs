using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

//using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Triggers;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

public partial class Test
{
	static MenuItem WpfMenuAddItem(ItemCollection m, string icon = null)
	{
		var a = new MenuItem();
		a.Header = "test";
		a.Click += A_Click;
		if(icon != null) {
			var ic = new Image { Source = new BitmapImage(new Uri(icon)) };
			a.Icon = ic;

			//a.Icon = icon; //displays text

			//IntPtr hi = Files.GetIconHandle(icon);
			//if(hi != Zero) {
			//	System.Drawing.Icon ic = System.Drawing.Icon.FromHandle(hi);
			//	a.Icon = ic.ToBitmap();
			//	ic.Dispose();
			//	Api.DestroyIcon(hi); //note: fails if this is immediately after 'Icon.FromHandle(hi)', although MSDN says need to call DestroyIcon() which implies that FromHandle() copies it.
			//}
		}
		m.Add(a);
		return a;
	}

	static void TestWpfContextMenu()
	{
		Perf.First();
		var m = new ContextMenu();
		//m.IsVisibleChanged += M_IsVisibleChanged;
		//m.Loaded += M_Loaded;
		//m.Opened += M_Opened;
		//m.Closed += M_Closed;
		m.Unloaded += M_Unloaded;
		Perf.Next();
		WpfMenuAddItem(m.Items);
		Perf.Next();
		for(int i = 0; i < 16; i++) {
			string icon = null;
			switch(i) {
			case 3: icon = @"q:\app\Cut.ico"; break;
			case 4: icon = @"q:\app\Copy.ico"; break;
			case 5: icon = @"q:\app\Paste.ico"; break;
			case 6: icon = @"q:\app\Run.ico"; break;
			case 7: icon = @"q:\app\Tip.ico"; break;
			case 8: icon = @"q:\app\Cut.ico"; break;
			case 9: icon = @"q:\app\Copy.ico"; break;
			case 10: icon = @"q:\app\Paste.ico"; break;
			case 11: icon = @"q:\app\Run.ico"; break;
			case 12: icon = @"q:\app\Tip.ico"; break;
			case 13: icon = @"q:\app\Cut.ico"; break;
			case 14: icon = @"q:\app\Copy.ico"; break;
				//case 15: icon = @"q:\app\qm.exe,1"; break; //exception
			}
			MenuItem k = WpfMenuAddItem(m.Items, icon);
			if(i == 3 || i == 4 || i == 14) {
				for(int j = 0; j < 9; j++) {
					MenuItem k2 = WpfMenuAddItem(k.Items);
					//for(int n = 0; n < 50; n++) { WpfMenuAddItem(k2.Items); }
				}
			}
		}
		Perf.Next();
		m.IsOpen = true;
		Perf.Next();
		test_mLoop.Loop();

		//Sleep(500);
		//Perf.First();
		//m.IsOpen = true;
		//Perf.Next();
		//test_mLoop.Loop();
	}

	static Util.MessageLoop test_mLoop = new Util.MessageLoop();

	private static void M_Unloaded(object sender, RoutedEventArgs e)
	{
		//PrintFunc();
		test_mLoop.Stop();
	}

	//private static void M_Closed(object sender, RoutedEventArgs e)
	//{
	//	PrintFunc();
	//	test_mLoop.Stop();
	//}

	//private static void M_Opened(object sender, RoutedEventArgs e)
	//{
	//	PrintFunc();
	//}

	//private static void M_Loaded(object sender, RoutedEventArgs e)
	//{
	//	PrintFunc();
	//}

	//private static void M_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	//{
	//	PrintFunc();
	//}

	private static void A_Click(object sender, RoutedEventArgs e)
	{
		PrintFunc();
	}





	static void TestWpfToolbar()
	{
		Perf.First();
		var t = new ToolBar();
		Perf.Next();

		for(int i = 0; i < 30; i++) {
			var b = new Button();
			b.Content = "Text";
			b.Click += B_Click;
			t.Items.Add(b);
			if(i == 0) Perf.Next();
		}
		Perf.Next();

		var p = new System.Windows.Controls.Primitives.Popup();
		p.Child = t;
		p.HorizontalOffset = 400; p.VerticalOffset = 200;
		Perf.Next();

		p.IsOpen = true;
		Perf.Next();

		//System.Windows.Forms.Application.Run();
		_mlTbWpf.Loop();
		p.IsOpen = false;
		Print("end");
	}

	static Util.MessageLoop _mlTbWpf = new Util.MessageLoop();

	private static void B_Click(object sender, RoutedEventArgs e)
	{
		PrintFunc();
		_mlTbWpf.Stop();
	}
}
