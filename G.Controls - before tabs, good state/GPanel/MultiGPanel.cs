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
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	public partial class GPanel :Panel
	{
		/// <summary>
		/// Auto-created GPanel that hosts other panels, making possible to have nested panels.
		/// </summary>
		public class MultiGPanel :GPanel
		{
			/// <summary>
			/// Called when another panel dropped on a side of target panel.
			/// Puts itself into the place of target panel. Makes target a child of self with Dock=Fill and hides its splitter.
			/// </summary>
			/// <param name="target"></param>
			internal MultiGPanel(GPanel target) : base(target._manager, target.Dock)
			{
				Debug.Assert(!target.IsFloating); //never mind, maybe later

				_floaty.CopyDockStateFrom(target._floaty);
				this.Visible = false;
				this.Bounds = target.Bounds;
				this.AddToParent(target.Parent);
				Wnd insAfter = (Wnd)target;
				((Wnd)this).ZorderAfter(insAfter);

				target.Visible = false;
				target.Parent = this;
				target.Dock = DockStyle.Fill;
				target.Visible = true;
				this.Visible = true;
			}

			protected override void OnControlRemoved(ControlEventArgs e)
			{
				var panel = e.Control as GPanel;
				if(panel != null) { //else splitter
					var a = _ChildPanels;
					if(a.Count == 0) { //removing last panel
						this.Hide();
					} else if(a.Count == 1) {
						panel = a[0];
						//panel._floaty.SavedDockStyle = panel.Dock;
						panel.Dock = DockStyle.Fill;
						//OutList("removed", panel._floaty.SavedDockStyle, panel.Name);
					}
				}

				base.OnControlRemoved(e);
			}

			internal void OnChildPanelDocked(GPanel panel)
			{
				//OutList("docked", panel.Name, _ChildPanels.Count);
				if(_ChildPanels.Count == 1) {
					//make DockStyle.Fill
					//panel._mdChildDockStyle = panel.Dock;
					panel.Dock = DockStyle.Fill;
					panel._OnZorderChanged();
					//OutList("docked single", panel._floaty.SavedDockStyle, panel.Name);
				} else if(_IsDockStyleLTRB(panel.Dock)) {
					//OutList("LRTB", panel._floaty.SavedDockStyle);
				} else {
					//make sure there are no multiple DockStyle.Fill panels
					foreach(var c in this.Controls.OfType<GPanel>()) {
						if(c == panel || _IsDockStyleLTRB(c.Dock)) continue;
						//OutList("docked multi", c._floaty.SavedDockStyle, c.Name);
						var dock = c._floaty.SavedDockStyle;
						Debug.Assert(_IsDockStyleLTRB(dock));
						c.Dock = dock;
						break;
					}
				}

				if(!this.Visible) this.Show();
			}

			List<GPanel> _ChildPanels
			{
				get
				{
					var a = new List<GPanel>();
					foreach(var c in this.Controls.OfType<GPanel>()) a.Add(c);
					return a;
				}
			}
		}
	}
}
