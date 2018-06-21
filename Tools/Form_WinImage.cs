//#define USE_CODEANALYSIS_REF

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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Controls;

#if USE_CODEANALYSIS_REF
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
#endif

namespace Au.Tools
{
	public partial class Form_WinImage :Form_
	{
		Wnd _wnd;
		//ToolsUtil.CaptureWindowEtcWithShift _capt;
		CommonInfos _commonInfos;

		public Form_WinImage()
		{
			InitializeComponent();
		}

		const string c_registryKey = @"\Tools\WinImage";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Wnd w = (Wnd)this;
			if(Registry_.GetString(out var wndPos, "wndPos", c_registryKey))
				try { w.RestorePositionSizeState(wndPos, true); } catch { }

			//_tWnd.WndVarNameChanged += (unu, sed) => _OnGridChanged();

			//_InitInfo();

			//_cCapture.Checked = true;

			//_capt = new ToolsUtil.CaptureWindowEtcWithShift(this, _Capture);
			//_capt.StartStop(true);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Wnd w = (Wnd)this;
			//_cCapture.Checked = false;
			//_capt?.Dispose();
			Registry_.SetString(w.SavePositionSizeState(), "wndPos", c_registryKey);
			base.OnFormClosing(e);
		}

		//RECT? _CaptureGetRect()
		//{
		//	var w = Wnd.FromMouse();
		//	if(w.GetClientRect(out var r, inScreen: true)) return r;
		//	return default;
		//}

		void _Capture()
		{
			//_capt.StartStop(false);
			//if(!WinImage.CaptureUI(out var r, 0, this)) { _capt.StartStop(true); return; }
			if(!WinImage.CaptureUI(out var r, 0, this)) return;
			_pict.BackColor = r.image == null ? (Color)r.color : SystemColors.ControlDarkDark;
			_pict.Image = r.image;
		}

		private void _bCapture_Click(object sender, EventArgs e)
		{
			_Capture();
		}
	}
}
