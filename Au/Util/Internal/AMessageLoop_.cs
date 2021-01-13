//This class is used in 1 place in this library: in AMenu when modal.
#define FORMS //FUTURE: remove if AMenu reimplemented without forms

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
//using System.Linq;
using System.Windows.Threading; //for XML comments

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// A message loop for non-WPF apps, alternative to <b>System.Windows.Forms.Application.Run</b> which does not support nested loops.
	/// In WPF apps instead use <see cref="DispatcherFrame"/> in a similar way; instead of <b>Loop</b> and <b>Stop</b> use <see cref="Dispatcher.PushFrame"/> and <see cref="DispatcherFrame.Continue"/>.
	/// </summary>
	class AMessageLoop_
	{
		Handle_ _loopEndEvent;

		/// <summary>
		/// Runs a message loop.
		/// </summary>
		public unsafe void Loop() {
#if FORMS
			bool isForms = 0 != (1 & Assembly_.IsLoadedWinformsWpf());
			using (isForms ? new EnsureWindowsFormsSynchronizationContext_(true) : null) {
#endif
				_loopEndEvent = Api.CreateEvent(true);
				try {
					_DoEvents();

					for (; ; ) {
						IntPtr ev = _loopEndEvent;
						int k = Api.MsgWaitForMultipleObjectsEx(1, &ev, 1000, Api.QS_ALLINPUT, Api.MWMO_INPUTAVAILABLE);
						if (k == Api.WAIT_TIMEOUT) continue; //previously timeout was used to support Thread.Abort. It is disabled in Core, but maybe still safer with a timeout.

						_DoEvents();
						if (k == Api.WAIT_OBJECT_0 || k == Api.WAIT_FAILED) break; //note: this is after DoEvents because may be posted messages when stopping loop. Although it seems that MsgWaitForMultipleObjects returns events after all messages.

						if (Api.PeekMessage(out var _, default, Api.WM_QUIT, Api.WM_QUIT, Api.PM_NOREMOVE)) break; //DoEvents() reposts it. If we don't break, MsgWaitForMultipleObjects retrieves it before (instead) the event, causing endless loop.
					}
				}
				finally {
					_loopEndEvent.Dispose();
				}
#if FORMS
			}

			void _DoEvents() {
				if (isForms) _DoEventsForms();
				else ATime.DoEvents();
				//info: with ATime.DoEvents something with forms does not work, eg keys/mouse with modal AMenu.
			}
#else
			void _DoEvents() => ATime.DoEvents();
#endif
		}

#if FORMS
		[MethodImpl(MethodImplOptions.NoInlining)] //avoid loading Forms + several other dlls
		void _DoEventsForms() => System.Windows.Forms.Application.DoEvents();
#endif

		/// <summary>
		/// Ends the message loop, causing <see cref="Loop"/> to return.
		/// </summary>
		public void Stop() {
			if (!_loopEndEvent.Is0) {
				Api.SetEvent(_loopEndEvent);
			}
		}
	}
}
