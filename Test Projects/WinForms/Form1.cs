using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace WinForms
{
	public partial class Form1 :Form
	{
		public Form1()
		{
			Perf.First();
			InitializeComponent();
			Perf.NW(); //9 ms empty, 10 ms with empty ListView, 98 ms with empty ObjectListView, 68 when all ngened, 51 with TreeListView

			//var a = new List<Data>();
			//a.Add(new Data("A", 1, false));
			//a.Add(new Data("B does it wrap?\r\nNew line.", 2, true));

			//treeListView1.SetObjects(a);

			var x = new _CustomizeTree(@"Q:\Test\CmdStrips.xml");

		}

		class _CustomizeTree
		{
			XDocument _doc;
			XElement _root;

			public _CustomizeTree(string xmlFile)
			{
				_doc = XDocument.Load(xmlFile);
				_root = _doc.Root;
			}

			class _Node
			{
				XElement _x;

				internal _Node(XElement x) { _x = x; }

				public int ChildCount { get => _x.Elements().Count(); }
				public List<_Node> Children
				{
					get
					{
						var e = _x.Elements();
						var r = new List<_Node>(e.Count());
						foreach(var v in e) r.Add(new _Node(v));
						return r;
					}
				}
				public string Label { get => _x.Name.ToString(); set { } }
				public _Node Parent { get; set; }
				public string ParentLabel { get => _x.Parent.Name.ToString(); }
			}
		}

		//protected override void OnActivated(EventArgs e)
		//{
		//	base.OnActivated(e);
		//	OutFunc();
		//}

		//protected override void OnShown(EventArgs e)
		//{
		//	base.OnShown(e);
		//	OutFunc();
		//}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//	base.OnPaint(e);
		//	//OutFunc();
		//	Time.SetTimer(1, true, t => Perf.NW());
		//}
	}

	class Data
	{
		public Data(string one, int two, bool three)
		{
			One = one; Two = two; Three = three;
		}
		public string One { get; set; }
		public int Two { get; set; }
		public bool Three { get; set; }
		public string Four { get; set; }
	}
}
