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

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	partial class GDockPanel
	{
		internal GAccContainer AccObj;

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			if(AccObj == null) AccObj = new GAccContainer(this, this);
			return AccObj;
		}

		partial class GFloat
		{
			internal GAccContainer AccObj;

			protected override AccessibleObject CreateAccessibilityInstance()
			{
				if(AccObj == null) AccObj = new GAccContainer(this, _manager);
				return AccObj;
			}

			internal string AccName { get { return _gc.AccName; } }
		}

		partial class GNode
		{
			GAccNode _acc;

			//internal GAccNode AccNode { get}
			public static explicit operator GAccNode(GNode gn) { return gn._acc ?? (gn._acc = new GAccNode(gn)); }

			internal virtual string AccName { get; }
			internal virtual string AccDescription { get; }

		}

		partial class GPanel
		{
			internal override string AccName { get { return Name; } }
			internal override string AccDescription { get { return ToolTipText; } }

		}

		partial class GTab
		{
			internal override string AccName { get { return "Tab group " + _manager._aTab.IndexOf(this); } }
			internal override string AccDescription { get { return string.Join(", ", this.Items.Select(v => v.AccName)); } }

		}

		partial class GSplit
		{
			internal override string AccName { get { return "Splitter " + _manager._aSplit.IndexOf(this); } }
			internal override string AccDescription { get { return (this.IsVerticalSplit ? "Vertical" : "Horizontal"); } }

		}

		/// <summary>
		/// Used by GDockPanel and GFloat.
		/// </summary>
		//[DebuggerStepThrough]
		internal class GAccContainer :ControlAccessibleObject
		{
			GDockPanel _manager;
			Control _control;
			GFloat _gf;

			public GAccContainer(Control ownerControl, GDockPanel manager) : base(ownerControl)
			{
				_manager = manager;
				_control = ownerControl;
				_gf = ownerControl as GFloat;

			}

			//public override AccessibleRole Role { get { return AccessibleRole.Client; } }

			public override string Name { get { return (_gf == null) ? "Panels" : _gf.AccName; } }

			public override AccessibleStates State { get { return AccessibleStates.None; } } //default is Focusable

			//public override Rectangle Bounds { get { return base.Bounds; } }

			public override AccessibleObject HitTest(int x, int y)
			{
				var p = _control.PointToClient(new Point(x, y));
				if(!_control.ClientRectangle.Contains(p)) return null;

				_HitTestData ht;
				switch(_manager._HitTest(_control, p.X, p.Y, out ht)) {
				case _HitTestResult.Splitter:
					return (GAccNode)ht.gs;
				case _HitTestResult.Caption:
					return (GAccNode)ht.ContentNode;
				}

				foreach(var gp in _manager._aPanel) {
					if(gp.ParentControl != _control || gp.IsHidden) continue;
					var c = gp.Content;
					if(!(c.Bounds.Contains(p) && c.Visible)) continue;
					var a1 = _AccOfControl(c);
					var a2 = a1?.HitTest(x, y);
					return a2 ?? a1;
				}

				//foreach(Control c in _control.Controls) {
				//	if(!(c.Bounds.Contains(p) && c.Visible)) continue;
				//	var a1 = _AccOfControl(c);
				//	var a2 = a1?.HitTest(x, y);
				//	return a2 ?? a1;
				//}

				return this;
			}

			AccessibleObject _AccOfControl(Control c)
			{
				AccessibleObject R = c.AccessibilityObject;
				if(R == null) return null;
				if(!(c is ToolStrip || c is Panel)) R = R.Parent; //need WINDOW, or will not include non-client area elements
				return R;
			}

			public override int GetChildCount()
			{
				return _manager._aPanel.Count * 2 + _manager._aTab.Count + _manager._aSplit.Count;
			}

			public override AccessibleObject GetChild(int index)
			{
				//note: we always get all GNode and controls (docked and floating), regardless of _control type (GDockPanel or GFloat).

				//Print(index);
				var n = _manager._aPanel.Count * 2;
				if(index < n) {
					var gp = _manager._aPanel[index / 2];
					if((index & 1) == 0) return (GAccNode)gp;
					return _AccOfControl(gp.Content);
				}
				index -= n;
				n = _manager._aTab.Count;
				if(index < n) return (GAccNode)_manager._aTab[index];
				index -= n;
				n = _manager._aSplit.Count;
				if(index < n) return (GAccNode)_manager._aSplit[index];
				Debug.Assert(false);
				return null;
			}

			//public override AccessibleObject Parent { get { return base.Parent; } }

			public override string DefaultAction
			{
				get
				{
					return base.DefaultAction;
				}
			}
		}

		/// <summary>
		/// AccessibleObject of GNode.
		/// We use separate class (don't inherit GNode from AccessibleObject) to avoid GNode cluttering and conflicts (Name, Bounds etc).
		/// </summary>
		class GAccNode :AccessibleObject
		{
			//GAccContainer _parentAcc; //will get from _gn.ParentControl, because it may change
			GNode _gn;

			internal GAccNode(GNode gn)
			{
				//PrintList("GAccNode ctor", gn.AccName);
				_gn = gn;
			}

			public override AccessibleRole Role
			{
				get
				{
					if(_gn is GSplit) return AccessibleRole.Separator;
					if(_gn is GTab) return AccessibleRole.PageTabList;
					return AccessibleRole.PageTab;
				}
			}

			public override string Name { get { return _gn.AccName; } }

			public override AccessibleStates State
			{
				get
				{
					if(!_gn.IsHidden) {
						var gs = _gn as GSplit;
						if(gs == null || gs.IsSplitterVisible) return AccessibleStates.None;
					}
					return AccessibleStates.Invisible;
				}
			}

			public override Rectangle Bounds
			{
				get
				{
					Rectangle r;
					GSplit gs; GPanel gp;
					if((gs = _gn as GSplit) != null) r = gs.SplitterBounds;
					else if((gp = _gn as GPanel) != null) r = gp.CaptionBounds;
					else r = (_gn as GTab).CaptionBoundsExceptButtons;
					return _gn.ParentControl.RectangleToScreen(r);
				}
			}

			public override AccessibleObject HitTest(int x, int y)
			{
				//Print("node.HitTest");
				if(this.Bounds.Contains(x, y)) return this;
				return null;
			}

			public override int GetChildCount() { return 0; }

			public override AccessibleObject Parent
			{
				get
				{
					var c = _gn.ParentControl;
					var gf = c as GFloat;
					if(gf != null) return gf.AccObj;
					return (c as GDockPanel).AccObj;
				}
			}

			public override string Description { get { return _gn.AccDescription; } }

			public override string DefaultAction
			{
				get
				{
					var gp = _gn as GPanel;
					if(gp != null && gp.IsTabbedPanel) return "Show panel";
					return null;
				}
			}

			public override void DoDefaultAction()
			{
				var gp = _gn as GPanel;
				var gt = gp?.ParentTab;
				if(gt != null) gt.SetActiveItem(gp);
			}

			public override AccessibleObject GetSelected()
			{
				var gt = _gn as GTab;
				if(gt != null) return (GAccNode)gt.ActiveItem;
				return null;
			}
		}
	}
}
