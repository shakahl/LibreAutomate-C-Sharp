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
			GPanel _panel1, _panel2;

			/// <summary>
			/// Called when panel2 dropped on panel1.
			/// Puts itself into the place of panel1 and puts panel1 and panel2 on itself.
			/// </summary>
			internal static void DroppedPanelOnPanel(GPanel panel1, GPanel panel2)
			{
				Debug.Assert(!panel1.IsFloating);

				//can reuse parent MultiGPanel of panel1?
				bool reuse = false;
				var m = panel1.Parent as MultiGPanel;
				if(m != null && m.ChildPanelCount == 1) {
					if(panel2 == m._panel1 || panel2 == m._panel2) reuse = true; //the dropped panel was a child of m
					else if(m._panel1.Parent == m) reuse = !m._panel2.IsFloating; //the other m previous child now docked somewhere else
					else if(m._panel2.Parent == m) reuse = !m._panel1.IsFloating; //the other m previous child now docked somewhere else
					Out(reuse);
				}
				if(reuse) { m._panel1 = panel1; m._panel2 = panel2; } else m = new MultiGPanel(panel1, panel2);

				panel2._floaty._dockState.parent = m;
				panel2._floaty.MakeDocked(true);
			}

			MultiGPanel(GPanel panel1, GPanel panel2) : base(panel1._manager, panel1.Dock)
			{
				_panel1 = panel1;
				_panel2 = panel2;

				_floaty._dockState = panel1._floaty._dockState;
				this.Visible = false;
				this.Bounds = panel1.Bounds;
				this.AddToParent(panel1.Parent);
				Wnd insAfter = (Wnd)panel1;
				((Wnd)this).ZorderAfter(insAfter);

				panel1.Visible = false;
				panel1.Parent = this;
				panel1.Dock = DockStyle.Fill;
				panel1.Visible = true;
				this.Visible = true;
			}

			protected override void OnControlRemoved(ControlEventArgs e)
			{
				var panel = e.Control as GPanel;
				if(panel != null) { //else splitter
					var a = ChildPanels;
					if(a.Length == 0) { //removing last panel
						this.Hide();
					} else if(a.Length == 1) {
						panel = a[0];
						//panel._floaty._dockState.dockStyle = panel.Dock;
						panel.Dock = DockStyle.Fill;
						//OutList("removed", panel._floaty._dockState.dockStyle, panel.Name);
					}
				}

				base.OnControlRemoved(e);
			}

			internal void OnChildPanelDocked(GPanel panel)
			{
				//OutList("docked", panel.Name, _ChildPanels.Count);
				if(ChildPanelCount == 1) {
					//make DockStyle.Fill
					//panel._mdChildDockStyle = panel.Dock;
					panel.Dock = DockStyle.Fill;
					panel._OnZorderChanged();
					//OutList("docked single", panel._floaty._dockState.dockStyle, panel.Name);
				} else if(_IsDockStyleLTRB(panel.Dock)) {
					//OutList("LRTB", panel._floaty._dockState.dockStyle);
				} else {
					//make sure there are no multiple DockStyle.Fill panels
					foreach(var c in this.Controls.OfType<GPanel>()) {
						if(c == panel || _IsDockStyleLTRB(c.Dock)) continue;
						//OutList("docked multi", c._floaty._dockState.dockStyle, c.Name);
						var dock = c._floaty._dockState.dockStyle;
						Debug.Assert(_IsDockStyleLTRB(dock));
						c.Dock = dock;
						break;
					}
				}

				if(!this.Visible) this.Show();
			}

			internal GPanel[] ChildPanels { get { return this.Controls.OfType<GPanel>().ToArray(); } }

			internal int ChildPanelCount { get { return this.Controls.OfType<GPanel>().Count(); } }

			internal int VisibleChildPanelCount { get { return this.Controls.OfType<GPanel>().Count(p=>p.Visible); } }
		}
	}
}
