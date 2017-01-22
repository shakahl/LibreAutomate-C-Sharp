using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

using F = System.Windows.Forms;

//using Xceed.Wpf.AvalonDock; //v2
//using AvalonDock; //v1.3
using ScintillaNET;

namespace Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow :Window
	{
		public MainWindow()
		{
			Perf.Next();
			InitializeComponent();

			////treeview
			//_tv = new CatTreeView(this);
			//_tv.Name = nameof(_tv);
			//_tv.ShowNodeToolTips = true;
			//_tv.HideSelection = false;
			//if(true) {
			//	_tv.LabelEdit = true;
			//} else {
			//	_tv.ShowPlusMinus = false;
			//	_tv.FullRowSelect = true;
			//	_tv.ShowLines = false;
			//	//_tv.SingleClickExpand //no such prop
			//}

			////code
			//_code = new Scintilla();
			//_code.Name = nameof(_code);
			////_code.Dock = DockStyle.Fill;

			Perf.Next();

			//Out(dockManager.DockableContents[0].Content);

			//var wfHost1 = dockManager.DockableContents[0].Content as F.Integration.WindowsFormsHost;
			//wfHost1.Child = _tv;

			//var wfHost2 = dockManager.ActiveDocument.Content as F.Integration.WindowsFormsHost;
			//wfHost2.Child = _code;

			Perf.Next();

			//var documentContent = new DocumentContent();
			//documentContent.Title = "MyNewContent";
			////documentContent.Content
			//documentContent.Show(dockManager);

			_LoadCollection();
			Perf.Next();

		}

		//protected override void OnInitialized(EventArgs e)
		//{
		//	base.OnInitialized(e);
		//	OutFunc();
		//}

		//protected override void OnActivated(EventArgs e)
		//{
		//	base.OnActivated(e);
		//	OutFunc();
		//}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			GC.Collect();
			//OutFunc();
			//Perf.NW();
			//Perf.Next();
			Time.SetTimer(1, true, t => Perf.NW());
		}
	}
}
