using System;
using System.Windows;
using System.Windows.Controls;

namespace Au.Controls
{
	public class KCheckBox : CheckBox
	{
		public new bool IsChecked {
			get => base.IsChecked == true;
			set => base.IsChecked = value;
		}

		public event RoutedEventHandler CheckChanged {
			add { base.Checked += value; base.Unchecked += value; }
			remove { base.Checked -= value; base.Unchecked -= value; }
		}
	}
}
