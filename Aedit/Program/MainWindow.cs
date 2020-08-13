using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;

using Au;
using Au.Types;
using Au.Controls.WPF;

namespace Aedit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		AuPanels _panels=new();

		public MainWindow() {
			InitializeComponent();

			_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", ProgramSettings.DirBS + "Layout.xml");
			_panels["Files"].Content = new TreeView();
			_panels["Output"].Content = new SciHost();
			_panels["Info"].Content = new TextBlock();
			_panels["Found"].Content = new SciHost();
			_panels["Document"].Content = new SciHost();
			_panels["Open"].Content = new ListBox();
			_panels["Running"].Content = new ListBox();
			this.Content = _panels.RootElem;
		}

		protected override void OnClosing(CancelEventArgs e) {
			_panels.Save();
			base.OnClosing(e);
		}
	}
}
