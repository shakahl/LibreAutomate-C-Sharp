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
//using System.Xml.Linq;

using Aga.Controls.Tree.NodeControls;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Aga.Controls.Tree
{
	partial class TreeViewAdv
	{
		internal AccContainer AccObj;

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			if(AccObj == null) AccObj = new AccContainer(this, this);
			return AccObj;
		}
	}

	class AccContainer :Control.ControlAccessibleObject
	{
		TreeViewAdv _tva;
		AccHeader _accHeader;

		public AccContainer(Control ownerControl, TreeViewAdv tva) : base(ownerControl)
		{
			_tva = tva;
		}

		public override AccessibleRole Role { get => AccessibleRole.List; }

		//public override string Name { get => ; } //inherits control WINDOW name

		//public override AccessibleStates State { get => AccessibleStates.Focusable; } //default is Focusable

		public override int GetChildCount()
		{
			int n = _tva.RowCount;
			if(_tva.UseColumns) n++;
			return n; //same as public ItemCount
		}

		public override AccessibleObject GetChild(int index)
		{
			//Print(index);
			if(_tva.UseColumns) {
				if(index == 0) return _AccHeader;
				index--;
			}
			return _tva.RowMap[index].Acc;
		}

		AccHeader _AccHeader { get => _accHeader ?? (_accHeader = new AccHeader(_tva)); }

		public override AccessibleObject HitTest(int x, int y)
		{
			var p = _tva.PointToClient(new Point(x, y));
			if(p.Y >= 0 && p.Y < _tva.ColumnHeaderHeight) return _AccHeader;
			var tn = _tva.GetNodeAt(p);
			if(tn != null) return tn.Acc;
			return this;
		}

		public override AccessibleObject GetFocused()
		{
			return _tva.CurrentNode?.Acc;
		}

		public override AccessibleObject GetSelected()
		{
			return _tva.SelectedNode?.Acc;
		}
	}


	class AccNode :AccessibleObject
	{
		TreeNodeAdv _tn;

		internal AccNode(TreeNodeAdv tn)
		{
			_tn = tn;
		}

		public override AccessibleRole Role { get => AccessibleRole.ListItem; }

		public override Rectangle Bounds
		{
			get
			{
				var tva = _tn.Tree;
				return tva.RectangleToScreen(tva.GetNodeBoundsInClient(_tn));
			}
		}

		public override AccessibleObject HitTest(int x, int y)
		{
			//Print("node.HitTest");
			if(this.Bounds.Contains(x, y)) return this;
			return null;
		}

		public override int GetChildCount() { return 0; }

		public override AccessibleObject Parent { get => _tn.Tree.AccObj; }

		public override AccessibleObject Navigate(AccessibleNavigation navdir)
		{
			switch(navdir) {
			case AccessibleNavigation.Next:
				return _tn.NextNode?.Acc;
			case AccessibleNavigation.Previous:
				return _tn.PreviousNode?.Acc;
			}
			return base.Navigate(navdir);
		}

		public override string Name
		{
			get
			{
				var tva = _tn.Tree;
				foreach(var c in tva.NodeControls) {
					if(c is BaseTextControl t) {
						var col = t.ParentColumn;
						if(col != null && !col.IsVisible) continue;
						return t.GetLabel(_tn);
					}
				}
				return null;
			}
		}

		public override string Description
		{
			get
			{
				var tva = _tn.Tree;
				using(new Au.Util.LibStringBuilder(out var b)) {
					b.Append(_tn.IsLeaf ? "item" : "folder");
					bool start = false;
					foreach(var c in tva.NodeControls) {
						var col = c.ParentColumn;
						if(col != null && !col.IsVisible) continue;
						switch(c) {
						case BaseTextControl t:
							if(!start) { start = true; continue; } //skip first text, it is used for Name
							if(col == null) b.AppendFormat(" | {0}", t.GetLabel(_tn));
							else b.AppendFormat(" | {0}: {1}", col.Header, t.GetLabel(_tn));
							break;
						case NodeCheckBox cb:
							b.AppendFormat(" | {0}", cb.GetValue(_tn));
							break;
						}
					}
					return b.ToString();
				}
			}
		}

		public override AccessibleStates State
		{
			get
			{
				var tva = _tn.Tree;
				var r = AccessibleStates.Selectable | AccessibleStates.Focusable;
				if(tva.SelectionMode != TreeSelectionMode.Single) r |= AccessibleStates.MultiSelectable;
				if(_tn.IsSelected) r |= AccessibleStates.Selected;
				if(_tn == tva.CurrentNode) r |= AccessibleStates.Focused;
				if(!_tn.IsVisible) r |= AccessibleStates.Invisible;
				if(!_tn.IsLeaf) r |= _tn.IsExpanded ? AccessibleStates.Expanded : AccessibleStates.Collapsed;
				if(_IsOffscreen()) r |= AccessibleStates.Offscreen;
				return r;
			}
		}

		bool _IsOffscreen()
		{
			var tva = _tn.Tree;
			int i = _tn.Index, first = tva.FirstVisibleRow;
			if(i < first) return true;
			int last = first + tva.RowLayout.PageRowCount;
			if(i < last) return false;
			if(i > last) return true;
			//possibly partially visible
			var r = tva.RowLayout.GetRowBounds(i);
			int y = r.Y + r.Height * 2 / 3;
			return y - tva.OffsetY > tva.DisplayRectangle.Height;
		}

		public override string DefaultAction { get => _tn.IsLeaf ? "Select" : (_tn.IsExpanded ? "Collapse" : "Expand"); }

		public override void DoDefaultAction()
		{
			if(_tn.IsLeaf) _tn.Tree.SelectedNode = _tn;
			else _tn.IsExpanded = !_tn.IsExpanded;
		}

		public override void Select(AccessibleSelection flags)
		{
			var tva = _tn.Tree;
			switch(flags) {
			case AccessibleSelection.TakeSelection:
			case AccessibleSelection.TakeFocus: //never mind: this is a flag that can be added to others
				tva.SelectedNode = _tn;
				break;
			case AccessibleSelection.AddSelection:
				if(tva.SelectionMode == TreeSelectionMode.Single) tva.ClearSelection();
				_tn.IsSelected = true;
				break;
			case AccessibleSelection.RemoveSelection:
				_tn.IsSelected = false;
				break;
				//case AccessibleSelection.ExtendSelection:
				//	//never mind
				//	break;
			}
		}
	}

	partial class TreeNodeAdv
	{
		AccNode _acc;

		internal AccNode Acc { get => _acc ?? (_acc = new AccNode(this)); }
	}


	class AccHeader :AccessibleObject
	{
		TreeViewAdv _tva;

		internal AccHeader(TreeViewAdv tva)
		{
			_tva = tva;
		}

		//never mind: should be LIST containgg children COLUMNHEADER. Now we just format column names in Description.
		public override AccessibleRole Role { get => AccessibleRole.ColumnHeader; }

		public override Rectangle Bounds
		{
			get
			{
				var r = _tva.GetColumnBounds(_tva.Columns.Count - 1);
				r = new Rectangle(0, 0, r.Right, _tva.ColumnHeaderHeight);
				r.X -= _tva.OffsetX;
				return _tva.RectangleToScreen(r);
			}
		}

		public override AccessibleObject HitTest(int x, int y)
		{
			if(this.Bounds.Contains(x, y)) return this;
			return null;
		}

		public override int GetChildCount() { return 0; }

		public override AccessibleObject Parent { get => _tva.AccObj; }

		public override AccessibleStates State { get => AccessibleStates.None; }

		public override string Name { get => "Header"; }

		public override string Description
		{
			get
			{
				using(new Au.Util.LibStringBuilder(out var b)) {
					foreach(var col in _tva.Columns) {
						if(!col.IsVisible) continue;
						if(b.Length > 0) b.Append(" | ");
						b.Append(col.Header);
					}
					return b.ToString();
				}
			}
		}
	}

}
