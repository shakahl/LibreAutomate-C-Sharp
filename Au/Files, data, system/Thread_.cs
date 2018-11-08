using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Thread functions. Extends <see cref="Thread"/>.
	/// </summary>
	public static class Thread_
	{
		/// <summary>
		/// Gets native thread id of this thread.
		/// Calls API <msdn>GetCurrentThreadId</msdn>.
		/// </summary>
		/// <remarks>
		/// It is not the same as <see cref="Thread.ManagedThreadId"/>.
		/// </remarks>
		/// <seealso cref="Wnd.ThreadId"/>
		public static int NativeId => Api.GetCurrentThreadId();

		/// <summary>
		/// Returns native thread handle of this thread.
		/// Calls API <msdn>GetCurrentThread</msdn>.
		/// </summary>
		/// <remarks>
		/// Don't need to close the handle.
		/// </remarks>
		public static IntPtr Handle => Api.GetCurrentThread();

		/// <summary>
		/// Returns true if this thread has a .NET message loop (Forms or WPF).
		/// </summary>
		/// <param name="isWPF">Has WPF message loop and no Forms message loop.</param>
		/// <remarks>
		/// Unlike calling <b>Application.MessageLoop</b> etc directly, this function does not cause to load Forms and WPF dlls.
		/// </remarks>
		/// <seealso cref="Wnd.GetWnd.ThreadWindows"/>
		public static bool HasMessageLoop(out bool isWPF)
		{
			//info: we don't call .NET functions directly to avoid loading assemblies.

			isWPF = false;
			int f = Util.Assembly_.LibIsLoadedFormsWpf();
			if(0 != (f & 1) && _HML_Forms()) return true;
			if(0 != (f & 2) && _HML_Wpf()) return isWPF = true;
			return false;
		}

		/// <inheritdoc cref="HasMessageLoop(out bool)"/>
		public static bool HasMessageLoop() => HasMessageLoop(out _);

		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool _HML_Forms() => System.Windows.Forms.Application.MessageLoop;
		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool _HML_Wpf() => System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null;

		/// <summary>
		/// Calls API OpenThread and TerminateThread.
		/// If it is a managed thread, at first need to set its IsBackground = true.
		/// </summary>
		/// <param name="nativeId"></param>
		internal static void LibTerminate(int nativeId)
		{
			var th = Api.OpenThread(Api.THREAD_TERMINATE, false, nativeId); if(th == default) return;
			Api.TerminateThread(th, 0);
			Api.CloseHandle(th);
		}

		/// <summary>
		/// Starts new thread: creates new <see cref="Thread"/> object, sets some properties and calls <see cref="Thread.Start"/>.
		/// Returns the <b>Thread</b> variable.
		/// </summary>
		/// <param name="threadProc">Thread procedure. Parameter <i>start</i> of <b>Thread</b> constructor.</param>
		/// <param name="background">If true (default), sets <see cref="Thread.IsBackground"/> = true.</param>
		/// <param name="sta">If true (default), calls <see cref="Thread.SetApartmentState"/>(ApartmentState.STA).</param>
		/// <param name="name">If not null, sets <see cref="Thread.Name"/> = name.</param>
		/// <exception cref="OutOfMemoryException"></exception>
		public static Thread Start(ThreadStart threadProc, bool background = true, bool sta = true, string name = null)
		{
			var t = new Thread(threadProc);
			if(background) t.IsBackground = true;
			if(sta) t.SetApartmentState(ApartmentState.STA);
			if(name != null) t.Name = name;
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
	}
}
