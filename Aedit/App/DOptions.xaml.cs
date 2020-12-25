using System;
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
using Au;

namespace Aedit
{
	/// <summary>
	/// Interaction logic for DOptions.xaml
	/// </summary>
	public partial class DOptions : Window
	{
		public DOptions() {
			InitializeComponent();
		}

		public static void ZShow() {
			if (s_dialog == null) {
				s_dialog = new DOptions { Owner = App.Wmain };
				s_dialog.Closed += (_, _) => s_dialog = null;
				s_dialog.Show();
			} else {
				s_dialog.Activate();
			}
		}
		static DOptions s_dialog;
	}
}