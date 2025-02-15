﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Au.Tests
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1() {
			InitializeComponent();
			var c = (Content as Grid).Children[0] as FlowDocumentScrollViewer;
			c.MouseLeftButtonUp += (_, _) => { print.it("c up"); };
			c.MouseLeftButtonUp += (_, _) => { print.it("c preview up"); };
			timer.after(600, _ => print.it("init", Keyboard.FocusedElement, _Focused(c), FocusManager.GetFocusScope(c)));
			c.ContextMenuOpening += (_, _) => {
				print.it("menu", Keyboard.FocusedElement, _Focused(c), FocusManager.GetFocusScope(c));
				timer.after(100, _ => print.it("100", Keyboard.FocusedElement, _Focused(c), FocusManager.GetFocusScope(c)));
				//timer.after(500, _ => Keyboard.Focus(c));
			};
			IInputElement _Focused(FrameworkElement e) => FocusManager.GetFocusedElement(FocusManager.GetFocusScope(e));
		}
	}
}
