using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Au.Tools
{
	/// <summary>
	/// Editable TextBox for Wnd.Find code.
	/// </summary>
	internal partial class TextBoxWnd :TextBox
	{
		public TextBoxWnd()
		{
			WndVar = "w";
		}

		/// <summary>
		/// Gets or sets the window handle.
		/// The setter formats Wnd.Find code and displays in this textbox.
		/// </summary>
		public Wnd Hwnd
		{
			get => _w;
			set
			{
				_w = value;

				var b = new StringBuilder("var ");
				b.Append(WndVar ?? "w").Append(" = +Wnd.Find(");

				var s = _w.Name;
				ToolsUtil.AppendStringArg(b, s, noComma: true);

				s = _w.ClassName;
				if(s == null) {
					this.Text = "invalid window handle";
					this.ReadOnly = true;
					return;
				}
				ToolsUtil.AppendStringArg(b, ToolsUtil.StripWndClassName(s, true));

				if(!_w.IsVisibleEx) ToolsUtil.AppendOtherArg(b, "WFFlags.HiddenToo", "flags");

				b.Append(");");

				this.ReadOnly = false;
				_noeventWndTextChanged = true;
				this.Text = b.ToString();
				_noeventWndTextChanged = false;
			}
		}
		Wnd _w;

		/// <summary>
		/// Gets the Wnd variable name in this textbox. Default "w".
		/// </summary>
		public string WndVar { get; private set; }

		//When the user edits this textbox, calls event WndVarNameChanged to update the window variable in the Find textbox.
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			if(_noeventWndTextChanged) return;
			var s = this.Text;
			if(s.RegexMatch_(@"^(?:Wnd|var) +(\w+) *=", 1, out var w) && w != WndVar) {
				WndVar = w;
				WndVarNameChanged?.Invoke(this, null);
			}
		}
		bool _noeventWndTextChanged;

		/// <summary>
		/// When the user changes the Wnd variable name in this textbox.
		/// Not called by the Hwnd property setter.
		/// </summary>
		public event EventHandler WndVarNameChanged;
	}
}
