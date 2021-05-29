using Au.Types;
using Au.Util;
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

namespace Au
{
	/// <summary>
	/// Thread functions.
	/// </summary>
	/// <seealso cref="Thread"/>
	public static class AThread
	{
		/// <summary>
		/// Gets native thread id of this thread.
		/// Calls API <msdn>GetCurrentThreadId</msdn>.
		/// </summary>
		/// <remarks>
		/// It is not the same as <see cref="Thread.ManagedThreadId"/>.
		/// </remarks>
		/// <seealso cref="AWnd.ThreadId"/>
		public static int Id => Api.GetCurrentThreadId();
		//speed: fast, but several times slower than Thread.CurrentThread.ManagedThreadId. Caching in a ThreadStatic variable makes even slower.

		/// <summary>
		/// Returns native thread handle of this thread.
		/// Calls API <msdn>GetCurrentThread</msdn>.
		/// </summary>
		/// <remarks>
		/// Don't need to close the handle.
		/// </remarks>
		public static IntPtr Handle => Api.GetCurrentThread();

		/// <summary>
		/// Returns true if this thread has a .NET message loop (winforms or WPF).
		/// </summary>
		/// <param name="isWPF">Has WPF message loop and no winforms message loop.</param>
		/// <seealso cref="AWnd.GetWnd.ThreadWindows"/>
		public static bool HasMessageLoop(out bool isWPF) {
			//info: we don't call .NET functions directly to avoid loading assemblies.

			isWPF = false;
			int f = Assembly_.IsLoadedWinformsWpf();
			if (0 != (f & 1) && _HML_Forms()) return true;
			if (0 != (f & 2) && _HML_Wpf()) return isWPF = true;
			return false;
		}

		///
		public static bool HasMessageLoop() => HasMessageLoop(out _);

		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool _HML_Forms() => System.Windows.Forms.Application.MessageLoop;
		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool _HML_Wpf() => System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null;

		///// <summary>
		///// Calls API OpenThread and TerminateThread.
		///// If it is a managed thread, at first need to set its IsBackground = true.
		///// </summary>
		///// <param name="nativeId"></param>
		//internal static void Terminate_(int nativeId)
		//{
		//	using var th = Api.OpenThread(Api.THREAD_TERMINATE, false, nativeId);
		//	if(!th.Is0) Api.TerminateThread(th, 0);
		//}

		/// <summary>
		/// Starts new thread: creates new <see cref="Thread"/> object, sets some properties and calls <see cref="Thread.Start"/>.
		/// Returns the <b>Thread</b> variable.
		/// </summary>
		/// <param name="threadProc">Thread procedure. Parameter <i>start</i> of <b>Thread</b> constructor.</param>
		/// <param name="background">
		/// If true (default), sets <see cref="Thread.IsBackground"/> = true.
		/// The process ends when the main thread and all foreground threads end; background threads then are terminated.
		/// </param>
		/// <param name="sta">If true (default), sets <see cref="ApartmentState.STA"/>.</param>
		/// <exception cref="OutOfMemoryException"></exception>
		public static Thread Start(Action threadProc, bool background = true, bool sta = true) {
			var t = new Thread(threadProc.Invoke);
			if (background) t.IsBackground = true;
			if (sta) t.SetApartmentState(ApartmentState.STA);
			t.Start();
			return t;
		}

		//rejected: rarely used.
		///// <summary>
		///// Starts new thread: creates new <see cref="Thread"/> object, sets some properties and calls <see cref="Thread.Start"/>.
		///// Returns the <b>Thread</b> variable.
		///// </summary>
		///// <param name="threadProc">Thread procedure. Parameter <i>start</i> of <b>Thread</b> constructor.</param>
		///// <param name="background">If true (default), sets <see cref="Thread.IsBackground"/> = true.</param>
		///// <param name="sta">If true (default), sets <see cref="Thread.SetApartmentState"/>(ApartmentState.STA).</param>
		///// <param name="name">If not null, sets <see cref="Thread.Name"/> = name.</param>
		///// <param name="stackSize">Parameter <i>maxStackSize</i> of <b>Thread</b> constructor.</param>
		///// <param name="parameter">Parameter <i>parameter</i> of <b>Thread.Start</b>.</param>
		///// <exception cref="Exception">Exceptions of <b>Thread</b> constructor and <see cref="Thread.Start"/>.</exception>
		//public static Thread Start(ParameterizedThreadStart threadProc, bool background = true, bool sta = true, string name = null, int stackSize = 0, object parameter = null)
		//{
		//	var t = new Thread(threadProc, stackSize);
		//	if(background) t.IsBackground = true;
		//	if(sta) t.SetApartmentState(ApartmentState.STA);
		//	if(name != null) t.Name = name;
		//	t.Start(parameter);
		//	return t;
		//}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetComApartment_(ApartmentState state) {
			var t = Thread.CurrentThread;
			t.TrySetApartmentState(ApartmentState.Unknown);
			t.TrySetApartmentState(state);

			//This is undocumented, but works if we set ApartmentState.Unknown at first.
			//With [STAThread] slower, and the process initially used to have +2 threads.
			//Speed when called to set STA at startup: 1.7 ms. If apphost calls OleInitialize, 1.5 ms.
			//tested: OleUninitialize in apphost does not make GetApartmentState return MTA.
		}
	}
}
