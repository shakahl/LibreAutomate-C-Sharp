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
	public class Thread_
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
		/// <seealso cref="Wnd.Misc.ThreadWindows"/>
		public static bool IsUI
		{
			get
			{
				if(System.Windows.Forms.Application.MessageLoop) return true;
				if(System.Windows.Threading.Dispatcher.FromThread(Thread.CurrentThread) != null) return true;
				return false;
			}
		}

	}
}
