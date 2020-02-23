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
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	/// <summary>
	/// A message loop, alternative to Application.Run which does not support nested loops.
	/// </summary>
	public class AMessageLoop
	{
		LibHandle _loopEndEvent;

		/// <summary>
		/// Runs a message loop.
		/// </summary>
		public unsafe void Loop()
		{
			bool isForms = 0 != (1 & LibAssembly.LibIsLoadedFormsWpf());
			using(isForms ? new LibEnsureWindowsFormsSynchronizationContext(true) : null) {
				_loopEndEvent = Api.CreateEvent(true);
				try {
					_DoEvents();

					for(; ; ) {
						IntPtr ev = _loopEndEvent;
						int k = Api.MsgWaitForMultipleObjectsEx(1, &ev, 1000, Api.QS_ALLINPUT, Api.MWMO_INPUTAVAILABLE);
						if(k == Api.WAIT_TIMEOUT) continue; //previously timeout was used to support Thread.Abort. It is disabled in Core, but maybe still safer with a timeout.

						_DoEvents();
						if(k == Api.WAIT_OBJECT_0 || k == Api.WAIT_FAILED) break; //note: this is after DoEvents because may be posted messages when stopping loop. Although it seems that MsgWaitForMultipleObjects returns events after all messages.

						if(Api.PeekMessage(out var _, default, Api.WM_QUIT, Api.WM_QUIT, Api.PM_NOREMOVE)) break; //DoEvents() reposts it. If we don't break, MsgWaitForMultipleObjects retrieves it before (instead) the event, causing endless loop.
					}
				}
				finally {
					_loopEndEvent.Dispose();
				}
			}

			void _DoEvents()
			{
				if(isForms) _DoEventsForms(); //info: with ATime.DoEvents something does not work, don't remember, maybe AMenu.
				else ATime.DoEvents();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)] //avoid loading Forms + several other dlls
		void _DoEventsForms() => Application.DoEvents();

		/// <summary>
		/// Ends the message loop, causing <see cref="Loop"/> to return.
		/// </summary>
		public void Stop()
		{
			if(!_loopEndEvent.Is0) {
				Api.SetEvent(_loopEndEvent);
			}
		}
	}
}
