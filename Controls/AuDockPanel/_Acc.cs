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

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	public partial class AuDockPanel
	{
		_AccContainer _acc;

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			if(_acc == null) _acc = new _AccContainer(this, this);
			return _acc;
		}

		partial class _Float
		{
			internal _AccContainer AccObj;

			protected override AccessibleObject CreateAccessibilityInstance()
			{
				if(AccObj == null) AccObj = new _AccContainer(this, _manager);
				return AccObj;
			}

			internal string AccName => _gc.AccName;
		}

		partial class _Node
		{
			_AccNode _acc;

			//internal _AccNode AccNode { get}
			public static explicit operator _AccNode(_Node gn) { return gn._acc ?? (gn._acc = new _AccNode(gn)); }

			internal virtual string AccName { get; }
			internal virtual string AccDescription { get; }

		}

		partial class _Panel
		{
			internal override string AccName => Name;
			internal override string AccDescription => ToolTipText;

		}

		partial class _Tab
		{
			internal override string AccName => "Tab group " + _manager._aTab.IndexOf(this);
			internal override string AccDescription => string.Join(", ", this.Items.Select(v => v.AccName));

		}

		partial class _Split
		{
			internal override string AccName => "Splitter " + _manager._aSplit.IndexOf(this);
			internal override string AccDescription => (this.IsVerticalSplit ? "Vertical" : "Horizontal");

		}

		/// <summary>
		/// Used by AuDockPanel and _Float.
		/// </summary>
		class _AccContainer :ControlAccessibleObject
		{
			AuDockPanel _manager;
			Control _control;
			_Float _gf;

			public _AccContainer(Control ownerControl, AuDockPanel manager) : base(ownerControl)
			{
				_manager = manager;
				_control = ownerControl;
				_gf = ownerControl as _Float;
			}

			//public override AccessibleRole Role => AccessibleRole.Client;

			public override string Name => (_gf == null) ? "Panels" : _gf.AccName;

			public override AccessibleStates State => AccessibleStates.None; //default is Focusable

			//public override Rectangle Bounds => base.Bounds;

			public override AccessibleObject HitTest(int x, int y)
			{
				var p = _control.PointToClient(new Point(x, y));
				if(!_control.ClientRectangle.Contains(p)) return null;

				_HitTestData ht;
				switch(_manager._HitTest(_control, p.X, p.Y, out ht)) {
				case _HitTestResult.Splitter:
					return (_AccNode)ht.gs;
				case _HitTestResult.Caption:
					return (_AccNode)ht.ContentNode;
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
				//note: we always get all _Node and controls (docked and floating), regardless of _control type (AuDockPanel or _Float).

				//Print(index);
				var n = _manager._aPanel.Count * 2;
				if(index < n) {
					var gp = _manager._aPanel[index / 2];
					if((index & 1) == 0) return (_AccNode)gp;
					return _AccOfControl(gp.Content);
				}
				index -= n;
				n = _manager._aTab.Count;
				if(index < n) return (_AccNode)_manager._aTab[index];
				index -= n;
				n = _manager._aSplit.Count;
				if(index < n) return (_AccNode)_manager._aSplit[index];
				Debug.Assert(false);
				return null;
			}

			//public override AccessibleObject Parent => base.Parent;

			//public override string DefaultAction => base.DefaultAction;
		}

		/// <summary>
		/// AccessibleObject of _Node.
		/// We use separate class (don't inherit _Node from AccessibleObject) to avoid _Node cluttering and conflicts (Name, Bounds etc).
		/// </summary>
		class _AccNode :AccessibleObject
		{
			//_AccContainer _parentAcc; //will get from _gn.ParentControl, because it may change
			_Node _gn;

			internal _AccNode(_Node gn)
			{
				//Print("_AccNode ctor", gn.AccName);
				_gn = gn;
			}

			public override AccessibleRole Role
			{
				get
				{
					if(_gn is _Split) return AccessibleRole.Separator;
					if(_gn is _Tab) return AccessibleRole.PageTabList;
					return AccessibleRole.PageTab;
				}
			}

			public override string Name => _gn.AccName;

			public override AccessibleStates State
			{
				get
				{
					if(!_gn.IsHidden) {
						if(!(_gn is _Split gs) || gs.IsSplitterVisible) return AccessibleStates.None;
					}
					return AccessibleStates.Invisible;
				}
			}

			public override Rectangle Bounds
			{
				get
				{
					Rectangle r;
					if(_gn is _Split gs) r = gs.SplitterBounds;
					else if(_gn is _Panel gp) r = gp.CaptionBounds;
					else r = (_gn as _Tab).CaptionBoundsExceptButtons;

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
					if(c is _Float gf) return gf.AccObj;
					return (c as AuDockPanel)._acc;
				}
			}

			public override string Description => _gn.AccDescription;

			public override string DefaultAction
			{
				get
				{
					if(_gn is _Panel gp && gp.IsTabbedPanel) return "Click";
					return null;
				}
			}

			public override void DoDefaultAction()
			{
				var gp = _gn as _Panel;
				var gt = gp?.ParentTab;
				gt?.SetActiveItem(gp, true);
			}

			public override AccessibleObject GetSelected()
			{
				if(_gn is _Tab gt) return (_AccNode)gt.ActiveItem;
				return null;
			}
		}
	}
}
