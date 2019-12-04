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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

#if TRACE
partial class ThisIsNotAFormFile { }

partial class PanelFiles
{
	public void Test()
	{
//		//Print(_c.SelectedNode);

//		//var name = "MSVCRT";
//		//var name = "msvcrt";
//		//var name = "Declarations";
//		//var name = "Backup";
//		var name = "QM3 fileS.cs";

//		//var n = _c.AllNodes.ElementAt(2);
//		var f = _model.Find(name, null);
//		//Print(f);
//		if(f == null) { Print("not found"); return; }
//		//var n = _c.FindNodeByTag(f);
//		var n = f?.TreeNodeAdv;
//		//Print(n?.Tag);

//		var m = new AMenu();
//		m["SelectedNode"] = o => _c.SelectedNode = n;
//		m["IsSelected=true"] = o => n.IsSelected = true; //adds to selection
//		m["ClearSelection"] = o => _c.ClearSelection();
//		m["ClearSelection; IsSelected=true"] = o => { _c.ClearSelection(); n.IsSelected = true; };
//		//m["ClearSelection; IsSelected=true"] = o => { _c.ClearSelection(); _c.SelectedNode=n; };
//		m["EnsureVisible"] = o => _c.EnsureVisible(n);
//		m["ScrollTo"] = o => _c.ScrollTo(n);
//		m["Find"] = o =>
//		{
//			Print(_model.Find("QM3 function flags.cs", false));
//			Print(_model.Find(@"\Function ideAs\AAcc.path.cs", false));
//			Print(_model.Find("mono compiler", false));
//			Print(_model.Find(@"\oLd\mono compiler", false));
//			Print("-----");
//			Print(_model.Find("QM3 function flags.cs", false));
//			Print(_model.Find(@"\Function ideAs\AAcc.path.cs", false));
//			Print(_model.Find("mono compiler", false));
//			Print(_model.Find(@"\oLd\mono compiler", false));
//			Print("-----");
//			Print(_model.Find("QM3 function flags.cs", true));
//			Print(_model.Find(@"\Function ideAs\AAcc.path.cs", true));
//			Print(_model.Find("mono compiler", true));
//			Print(_model.Find(@"\oLd\mono compiler", true));
//		};
//		m["FullUpdate"] = o => _c.FullUpdate();
//		m["HideEditor"] = o => _c.HideEditor();
//		//m["Delete"] = o => f.FileDelete();
//		//m["Clear"] = o => _model.Clear();
//		m["StructureChanged"] = o => _model.OnStructureChanged();
//		m["Add"] = o =>
//		{
//		};
//		m["GetNodeBounds"] = o =>
//		{
//			Print(_c.GetNodeBounds(n));
//			Print(_c.GetNodeBoundsInClient(n));
//		};
//		m["GetNodeControls"] = o =>
//		{
//			int dx = _c.OffsetX, dy = _c.OffsetY;
//			foreach(var v in _c.GetNodeControls(n)) {
//				var r = v.Bounds; r.X -= dx; r.Y -= dy;
//				Print(r);
//			}
//		};
//		m["GetNodeControlInfoAt"] = o =>
//		{
//			var k = _c.GetNodeControlInfoAt(new Point(60, 40));
//			Print(k.Node?.Tag, k.Bounds);
//		};
//#if TEST_MANY_COLUMNS
//				m["column IsVisible"] = o =>
//				{
//					_columnGUID.IsVisible = !_columnGUID.IsVisible;
//				};
//#endif
//		m["speed"] = o =>
//		{
//			APerf.First();
//			//n.IsSelected = true;
//			_c.Invalidate(false);
//			APerf.Next();
//			_c.Update();
//			APerf.NW();
//		};
//		m["change cell"] = o =>
//		{
//			f.FileRename("new looooooooooooooooooooooooooooooong naaaaaaaame");
//			//f.GUID = "one two";
//			//f.GUID = "one\ntwo";
//			APerf.First();
//			_c.Invalidate(false); //now fast, but Update slow
//								  //_model.OnNodeChanged(f); //now quite slow, and Update slow
//			APerf.Next();
//			_c.Update();
//			APerf.NW();

//			ATimer.After(1000, _ => f.FileRename(name));
//		};
//		m["udate row"] = o =>
//		{
//			f.FileRename("new looooooooooooooooooooooooooooooong naaaaaaaame");
//			//f.GUID = "one two";
//			APerf.First();
//			_c.UpdateNode(n);
//			APerf.Next();
//			_c.Update();
//			APerf.NW();

//			ATimer.After(1000, _ => f.FileRename(name));
//		};
//		m["GC.Collect"] = o => GC.Collect();
//		m["disable control"] = o =>
//		{
//			_c.Enabled = false;
//			ADialog.Show();
//			_c.Enabled = true;
//		};
//		m["ItemPath"] = o =>
//		{
//			f = _model.Find("obsolete", null);
//			Print(f?.ItemPath);
//			f = _model.Find("Zip_to_WEB", null);
//			Print(f?.ItemPath);
//			f = _model.Find("tips.txt", null);
//			Print(f?.ItemPath);
//		};
//		m["FilePath"] = o =>
//		{
//			f = _model.Find("obsolete", null);
//			Print(f?.FilePath);
//			f = _model.Find("Zip_to_WEB", null);
//			Print(f?.FilePath);
//			f = _model.Find("tips.txt", null);
//			Print(f?.FilePath);
//		};
//		m["delete icon cache"] = o => { FileNode.IconCache.ClearCache(); _c.Invalidate(); };
//		m.Separator();

//		m.Show();
	}
}
#endif
