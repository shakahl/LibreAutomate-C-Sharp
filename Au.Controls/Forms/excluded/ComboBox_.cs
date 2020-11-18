using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Au.Types;
using System.Windows.Forms;

namespace Au.Util
{
	/// <summary>
	/// Fixes some ComboBox bugs.
	/// </summary>
	internal class ComboBox_ : ComboBox
	{
		protected override void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(out string s, m); AOutput.Write("<><c green>"+s+"<>");

			//workaround for native ComboBox control bug: on resize draws its edit control as if it is focused.
			//	Even worse, may change its text.
			//	https://stackoverflow.com/questions/2151447/odd-combobox-behavior-on-resize
			if(m.Msg == WM_SIZE) {
				var edit = ((AWnd)m.HWnd).Get.FirstChild;
				Native.SetWindowSubclass(edit, s_subclass1, 1, default);
				try { base.WndProc(ref m); }
				finally { Native.RemoveWindowSubclass(edit, s_subclass1, 1); }
				return;
			}

			base.WndProc(ref m);
		}

		static LPARAM _Subclass1(AWnd hWnd, int msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, IntPtr dwRefData)
		{
			//AWnd.More.PrintMsg(hWnd, msg, wParam, lParam);
			switch(msg) { case WM_SETTEXT: case EM_SETSEL: return 0; }
			return Native.DefSubclassProc(hWnd, msg, wParam, lParam);
		}
		static Native.SUBCLASSPROC s_subclass1 = _Subclass1;

		const int WM_SIZE = 0x5;
		const int WM_SETTEXT = 0xC;
		const int EM_SETSEL = 0xB1;

		//workaround for .NET ComboBox bug: setting Text or SelectedIndex creates parked handle.
		string _text;
		int _index = -1;

		public override string Text {
			get => IsHandleCreated ? base.Text : _text;
			set {
				if(IsHandleCreated) base.Text = value;
				else _text = value;
			}
		}

		public override int SelectedIndex {
			get => IsHandleCreated ? base.SelectedIndex : _index;
			set {
				if(IsHandleCreated) base.SelectedIndex = value;
				else _index = value;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if(_index >= 0) base.SelectedIndex = _index;
			else if(_text != null) base.Text = _text;
			_text = null;
			_index = -1;
		}
	}
}
