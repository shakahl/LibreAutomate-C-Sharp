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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

#if TEST
partial class ThisIsNotAFormFile { }

partial class PanelFiles
{
	public void Test()
	{
		//Print(_c.SelectedNode);

		//var name = "MSVCRT";
		//var name = "msvcrt";
		//var name = "Declarations";
		//var name = "Backup";
		var name = "Catkeys fileS.cs";

		//var n = _c.AllNodes.ElementAt(2);
		var f = _model.FindFileOrFolder(name);
		//Print(f);
		if(f == null) { Print("not found"); return; }
		//var n = _c.FindNodeByTag(f);
		var n = f?.TreeNodeAdv;
		//Print(n?.Tag);

		var m = new CatMenu();
		m["SelectedNode"] = o => _c.SelectedNode = n;
		m["IsSelected=true"] = o => n.IsSelected = true; //adds to selection
		m["ClearSelection"] = o => _c.ClearSelection();
		m["ClearSelection; IsSelected=true"] = o => { _c.ClearSelection(); n.IsSelected = true; };
		//m["ClearSelection; IsSelected=true"] = o => { _c.ClearSelection(); _c.SelectedNode=n; };
		m["EnsureVisible"] = o => _c.EnsureVisible(n);
		m["ScrollTo"] = o => _c.ScrollTo(n);
		m["Find"] = o =>
		{
			Print(_model.FindFileOrFolder("QM3 function flags.cs"));
			Print(_model.FindFileOrFolder(@"\Function ideAs\Acc.path.cs"));
			Print(_model.FindFileOrFolder("mono compiler"));
			Print(_model.FindFileOrFolder(@"\oLd\mono compiler"));
			Print("-----");
			Print(_model.FindFile("QM3 function flags.cs"));
			Print(_model.FindFile(@"\Function ideAs\Acc.path.cs"));
			Print(_model.FindFile("mono compiler"));
			Print(_model.FindFile(@"\oLd\mono compiler"));
			Print("-----");
			Print(_model.FindFolder("QM3 function flags.cs"));
			Print(_model.FindFolder(@"\Function ideAs\Acc.path.cs"));
			Print(_model.FindFolder("mono compiler"));
			Print(_model.FindFolder(@"\oLd\mono compiler"));
		};
		m["FullUpdate"] = o => _c.FullUpdate();
		m["HideEditor"] = o => _c.HideEditor();
		m["Delete"] = o => f.FileDelete();
		//m["Clear"] = o => _model.Clear();
		m["StructureChanged"] = o => _model.OnStructureChanged();
		m["Add"] = o =>
		{
#if false
					var x = new XElement("f", new XAttribute("n", "NEW"));
#else
			var x = new XElement("d", new XAttribute("n", "NEW DIR"));
			x.Add(new XElement("f", new XAttribute("n", "NEW FILE")));
#endif
			var k = new FileNode(_model, x);
			//f.AddChildOrSibling(k);
			f.AddChildOrSibling(k, NodePosition.After);
		};
		m["GetNodeBounds"] = o =>
		{
			Print(_c.GetNodeBounds(n));
			Print(_c.GetNodeBoundsInClient(n));
		};
		m["GetNodeControls"] = o =>
		{
			int dx = _c.OffsetX, dy = _c.OffsetY;
			foreach(var v in _c.GetNodeControls(n)) {
				var r = v.Bounds; r.X -= dx; r.Y -= dy;
				PrintList(r);
			}
		};
		m["GetNodeControlInfoAt"] = o =>
		{
			var k = _c.GetNodeControlInfoAt(new Point(60, 40));
			PrintList(k.Node?.Tag, k.Bounds);
		};
#if TEST_MANY_COLUMNS
				m["column IsVisible"] = o =>
				{
					_columnGUID.IsVisible = !_columnGUID.IsVisible;
				};
#endif
		m["speed"] = o =>
		{
			Perf.First();
			//n.IsSelected = true;
			_c.Invalidate(false);
			Perf.Next();
			_c.Update();
			Perf.NW();
		};
		m["change cell"] = o =>
		{
			f.FileRename("new looooooooooooooooooooooooooooooong naaaaaaaame", false);
			//f.GUID = "one two";
			//f.GUID = "one\ntwo";
			Perf.First();
			_c.Invalidate(false); //now fast, but Update slow
								  //_model.OnNodeChanged(f); //now quite slow, and Update slow
			Perf.Next();
			_c.Update();
			Perf.NW();

			Timer_.After(1000, t => f.FileRename(name, true));
		};
		m["udate row"] = o =>
		{
			f.FileRename("new looooooooooooooooooooooooooooooong naaaaaaaame", false);
			//f.GUID = "one two";
			Perf.First();
			_c.UpdateNode(n);
			Perf.Next();
			_c.Update();
			Perf.NW();

			Timer_.After(1000, t => f.FileRename(name, true));
		};
		m["GC.Collect"] = o => GC.Collect();
		m["disable control"] = o =>
		{
			_c.Enabled = false;
			TaskDialog.Show();
			_c.Enabled = true;
		};
		m["ItemPath"] = o =>
		{
			f = _model.FindFileOrFolder("obsolete");
			Print(f?.ItemPath);
			f = _model.FindFileOrFolder("Zip_to_WEB");
			Print(f?.ItemPath);
			f = _model.FindFileOrFolder("tips.txt");
			Print(f?.ItemPath);
		};
		m["FilePath"] = o =>
		{
			f = _model.FindFileOrFolder("obsolete");
			Print(f?.FilePath);
			f = _model.FindFileOrFolder("Zip_to_WEB");
			Print(f?.FilePath);
			f = _model.FindFileOrFolder("tips.txt");
			Print(f?.FilePath);
		};
		m["delete icon cache"] = o => { FilesModel.IconCache.ClearCache(); _c.Invalidate(); };
		m.Separator();

		m.Show();
	}
}
#endif
