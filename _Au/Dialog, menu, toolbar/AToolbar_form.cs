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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;

namespace Au
{
	public partial class AToolbar
	{
		/// <summary>
		/// Creates new form with a list of toolbars of this thread. Can be used to find lost toolbars.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// AToolbar.NewToolbarsForm().Show();
		/// ]]></code>
		/// </example>
		public static Form NewToolbarsForm() => new _Form();

		class _Form : Form
		{
			public _Form()
			{
				InitializeComponent();

				_FillLB();

				_bShow.Click += _bClick;
				_bMove.Click += _bClick;
				_bRefresh.Click += (_, __) => {
					_lb.Items.Clear();
					_FillLB();
				};

				void _FillLB()
				{
					foreach(var tb in _Manager._atb) {
						_lb.Items.Add(tb);
						var sat = tb.Satellite; if(sat != null && !sat._c.Hwnd().Is0) _lb.Items.Add(sat);
					}
				}
			}

			private void _bClick(object sender, EventArgs e)
			{
				if(!(_lb.SelectedItem is AToolbar tb)) return;
				if(tb._c.IsDisposed) {
					ADialog.Show("Closed", owner: this);
					return;
				}
				var w = tb._c.Hwnd();
				var r = w.Rect;
				if(sender == _bShow) {
					if(AScreen.IsInAnyScreen(r)) {
						r.Inflate(2, 2);
						var osd = new AOsdRect { Rect = r, Color = 0xff0000 };
						osd.Show();
						ATimer.After(1000, _ => osd.Dispose());
					} else {
						ADialog.Show("Offscreen", "Rectangle: " + r.ToString(), owner: this);
					}
				} else if(sender == _bMove) {
					if(!w.IsVisible && !ADialog.ShowOKCancel("Hidden", "Move this hidden toolbar?", owner: this)) return;
					var xy = AMouse.XY;
					w.MoveLL(xy.x, xy.y);
					var w2 = this.Hwnd();
					if(!w.ZorderIsAbove(w2)) w.ZorderAbove(w2);
				}
			}

			protected override void OnLoad(EventArgs e)
			{
				Api.SetForegroundWindow(this.Hwnd());
				base.OnLoad(e);
			}

			#region Windows Form Designer generated code

			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this._lb = new System.Windows.Forms.ListBox();
				this._bShow = new System.Windows.Forms.Button();
				this._bMove = new System.Windows.Forms.Button();
				this._bRefresh = new System.Windows.Forms.Button();
				this.SuspendLayout();
				// 
				// _lb
				// 
				this._lb.Location = new System.Drawing.Point(0, 0);
				this._lb.Name = "_lb";
				this._lb.Size = new System.Drawing.Size(172, 199);
				this._lb.TabIndex = 0;
				// 
				// _bShow
				// 
				this._bShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
				this._bShow.Location = new System.Drawing.Point(178, 0);
				this._bShow.Name = "_bShow";
				this._bShow.Size = new System.Drawing.Size(106, 24);
				this._bShow.TabIndex = 1;
				this._bShow.Text = "Show rectangle";
				// 
				// _bMove
				// 
				this._bMove.FlatStyle = System.Windows.Forms.FlatStyle.System;
				this._bMove.Location = new System.Drawing.Point(178, 28);
				this._bMove.Name = "_bMove";
				this._bMove.Size = new System.Drawing.Size(106, 24);
				this._bMove.TabIndex = 2;
				this._bMove.Text = "Move here";
				// 
				// _bRefresh
				// 
				this._bRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
				this._bRefresh.Location = new System.Drawing.Point(178, 168);
				this._bRefresh.Name = "_bRefresh";
				this._bRefresh.Size = new System.Drawing.Size(106, 24);
				this._bRefresh.TabIndex = 3;
				this._bRefresh.Text = "Refresh";
				// 
				// _Form
				// 
				this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(291, 199);
				this.Controls.Add(this._bMove);
				this.Controls.Add(this._bShow);
				this.Controls.Add(this._bRefresh);
				this.Controls.Add(this._lb);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				this.Text = "Toolbars";
				this.ResumeLayout(false);

			}

			#endregion

			private System.Windows.Forms.ListBox _lb;
			private System.Windows.Forms.Button _bShow;
			private System.Windows.Forms.Button _bMove;
			private System.Windows.Forms.Button _bRefresh;
		}
	}
}
