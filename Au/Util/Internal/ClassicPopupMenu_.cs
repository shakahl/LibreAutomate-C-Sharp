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
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	class ClassicPopupMenu_ : IDisposable
	{
		IntPtr _h;

		public ClassicPopupMenu_()
		{
			_h = Api.CreatePopupMenu();
		}

		public void Dispose()
		{
			if(_h != default) { Api.DestroyMenu(_h); _h = default; }
			GC.SuppressFinalize(this);
		}

		~ClassicPopupMenu_() => Dispose();

		public void Add(int id, string text) => Api.AppendMenu(_h, 0, id, text);

		public void Separator() => Api.AppendMenu(_h);

		public int Show(AWnd owner)
		{
			if(Api.GetFocus() == default) { //prevent activating the menu window on click
				ATimer.After(1, _ => {
					var w = AWnd.Find("", "#32768", WOwner.ThisThread); //find visible classic menu window of this thread
					if(!w.Is0) w.SetExStyle(WS2.NOACTIVATE, WSFlags.Add);
				});
			}
			var p = AMouse.XY;
			return Api.TrackPopupMenuEx(_h, Api.TPM_RETURNCMD, p.x, p.y, owner);
		}
	}
}
