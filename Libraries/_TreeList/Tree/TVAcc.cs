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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Aga.Controls.Tree.NodeControls;

using Au;
using Au.Types;

namespace Aga.Controls.Tree
{
	partial class TreeViewAdv
	{
		internal _AccContainer AccObj;

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			if(AccObj == null) AccObj = new _AccContainer(this);
			return AccObj;
		}

		/// <summary>
		/// Max count of accessible objects that can be created for items.
		/// Default 1000.
		/// </summary>
		/// <remarks>
		/// Controls with large number of visible items consume much memory for accessible objects, because of very inefficient accessibility implementation of .NET. For example 120 MB of physical memory for 10000 items. Luckily accessible objects are created only when/if some accessibility/automation/etc app wants to use them.
		/// This property limits the number of accessible objects when some app wants to get all objects, but not when wants to get object from point or the focused/selected object.
		/// </remarks>
		public int ZAccessibleCount { get; set; } = 1000;

		//Tried to implement so that items would have only childID, not IAccessible. Failed. Although the same algorithm works well in C++, tested.
		//At first tried to implement (override) IAccessible (defined by .NET, need reference Accessibility).
		//	Our methods are called OK, but clients cannot get children. Probably because .NET implements IEnumVARIANT which gets 0 children. Failed to override IEnumVARIANT; it's internal, and our IEnumVARIANT is ignored. Tried ICustomQueryInterface but it does not work.
		//Then tried to override WM_GETOBJECT, like I would do it in C++, and like .NET does.
		//	It works well if client calls AccesibleObjectFromWindow(OBJID_CLIENT). But if calls OBJID_WINDOW and AccessibleChildren, somehow the CLIENT object is not ours.
		//The source codes are in QM2 macros TVAcc.csX.
		//Tested how much memory for AO uses DataGrid control with the same number of items. More than 2 times more. It implements AO for rows and cells, therefore the AO count is *2.
	}

	class _AccContainer : Control.ControlAccessibleObject
	{
		TreeViewAdv _tva;
		_AccHeader _accHeader;

		public _AccContainer(TreeViewAdv tva) : base(tva)
		{
			_tva = tva;
		}

		public override AccessibleRole Role => AccessibleRole.List;

		public override int GetChildCount()
		{
			int n = Math.Min(_tva.RowCount, _tva.ZAccessibleCount);
			if(_tva.UseColumns) n++;
			return n;
		}

		public override AccessibleObject GetChild(int index)
		{
			//AOutput.Write(index);
			if(_tva.UseColumns) {
				if(index == 0) return _AccHeader;
				index--;
			}
			return _tva.RowMap[index].Acc;
		}

		_AccHeader _AccHeader => _accHeader ?? (_accHeader = new _AccHeader(_tva));

		public override AccessibleObject HitTest(int x, int y)
		{
			var p = _tva.PointToClient(new Point(x, y));
			if(!_tva.ClientRectangle.Contains(p)) return null;
			if(p.Y < _tva.ColumnHeaderHeight) return _AccHeader;
			var tn = _tva.GetNodeAt(p);
			if(tn != null) return tn.Acc;
			return this;
		}

		public override AccessibleObject GetFocused() => _tva.CurrentNode?.Acc;

		public override AccessibleObject GetSelected() => _tva.SelectedNode?.Acc;
	}


	class _AccNode : AccessibleObject
	{
		TreeNodeAdv _tn;

		internal _AccNode(TreeNodeAdv tn)
		{
			_tn = tn;
		}

		public override AccessibleRole Role => AccessibleRole.ListItem;

		public override Rectangle Bounds {
			get {
				var tva = _tn.Tree;
				return tva.RectangleToScreen(tva.GetNodeBoundsInClient(_tn));
			}
		}

		public override AccessibleObject HitTest(int x, int y)
		{
			//AOutput.Write("node.HitTest");
			if(this.Bounds.Contains(x, y)) return this;
			return null;
		}

		public override int GetChildCount() => 0;

		public override AccessibleObject Parent => _tn.Tree.AccObj;

		public override AccessibleObject Navigate(AccessibleNavigation navdir)
		{
			switch(navdir) {
			case AccessibleNavigation.Next: return _tn.NextNode?.Acc;
			case AccessibleNavigation.Previous: return _tn.PreviousNode?.Acc;
			}
			return base.Navigate(navdir);
		}

		public override string Name {
			get {
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

		public override string Description {
			get {
				var tva = _tn.Tree;
				using(new Au.Util.StringBuilder_(out var b)) {
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

		public override AccessibleStates State {
			get {
				var tva = _tn.Tree;
				var r = AccessibleStates.Selectable | AccessibleStates.Focusable;
				if(tva.SelectionMode != TreeSelectionMode.Single) r |= AccessibleStates.MultiSelectable;
				if(_tn.IsSelected) r |= AccessibleStates.Selected;
				if(_tn == tva.CurrentNode) r |= AccessibleStates.Focused;
				if(!_tn.IsVisible) r |= AccessibleStates.Invisible;
				if(!_tn.IsLeaf) r |= _tn.IsExpanded ? AccessibleStates.Expanded : AccessibleStates.Collapsed;
				//if(_IsOffscreen()) r |= AccessibleStates.Offscreen;
				return r;
			}
		}

		//rejected
		//bool _IsOffscreen()
		//{
		//	var tva = _tn.Tree;
		//	int i = _tn.Row, first = tva.FirstVisibleRow;
		//	if(i < first) return true;
		//	int last = first + tva.RowLayout.PageRowCount;
		//	if(i < last) return false;
		//	if(i > last) return true;
		//	//possibly partially visible
		//	var r = tva.RowLayout.GetRowBounds(i);
		//	int y = r.Y + r.Height * 2 / 3;
		//	return y - tva.OffsetY > tva.DisplayRectangle.Height;
		//}

		public override string DefaultAction => _tn.IsLeaf ? "Select" : (_tn.IsExpanded ? "Collapse" : "Expand");

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
		_AccNode _acc;

		internal _AccNode Acc => _acc ??= new _AccNode(this);
	}


	class _AccHeader : AccessibleObject
	{
		TreeViewAdv _tva;

		internal _AccHeader(TreeViewAdv tva)
		{
			_tva = tva;
		}

		//never mind: should be LIST containing children COLUMNHEADER. Now we just format column names in Description.
		public override AccessibleRole Role => AccessibleRole.ColumnHeader;

		public override Rectangle Bounds {
			get {
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

		public override int GetChildCount() => 0;

		public override AccessibleObject Parent => _tva.AccObj;

		public override AccessibleStates State => AccessibleStates.None;

		public override string Name => "Header";

		public override string Description {
			get {
				using(new Au.Util.StringBuilder_(out var b)) {
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
