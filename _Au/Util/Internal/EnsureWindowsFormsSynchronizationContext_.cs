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
	/// Constructor ensures that current SynchronizationContext of this thread is WindowsFormsSynchronizationContext.
	/// Also sets WindowsFormsSynchronizationContext.AutoInstall=false to prevent Application.DoEvents etc setting wrong context.
	/// Dispose() restores both if need. Does not restore context if was null.
	/// Example: using(new Util.EnsureWindowsFormsSynchronizationContext_()) { ... }
	/// </summary>
	class EnsureWindowsFormsSynchronizationContext_ : IDisposable
	{
		[ThreadStatic] static WindowsFormsSynchronizationContext t_wfContext;
		SynchronizationContext _prevContext;
		bool _restoreContext, _prevAutoInstall;

		/// <summary>
		/// See class help.
		/// </summary>
		/// <param name="onlyIfAutoInstall">Do nothing if WindowsFormsSynchronizationContext.AutoInstall==false. Normally WindowsFormsSynchronizationContext.AutoInstall is true (default).</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public EnsureWindowsFormsSynchronizationContext_(bool onlyIfAutoInstall = false)
		{
			if(onlyIfAutoInstall && !WindowsFormsSynchronizationContext.AutoInstall) return;

			//Ensure WindowsFormsSynchronizationContext for this thread.
			_prevContext = SynchronizationContext.Current;
			if(!(_prevContext is WindowsFormsSynchronizationContext)) {
				t_wfContext ??= new WindowsFormsSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(t_wfContext);
				_restoreContext = _prevContext != null;
			}

			//Workaround for Application.DoEvents/Run bug:
			//	When returning, they set a SynchronizationContext of wrong type, even if previously was WindowsFormsSynchronizationContext.
			//	They don't do it if WindowsFormsSynchronizationContext.AutoInstall == false.
			//	Info: AutoInstall is [ThreadStatic], as well as SynchronizationContext.Current.
			_prevAutoInstall = WindowsFormsSynchronizationContext.AutoInstall;
			if(_prevAutoInstall) WindowsFormsSynchronizationContext.AutoInstall = false;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Dispose()
		{
#if DEBUG
			_disposed = true;
#endif
			if(_restoreContext) {
				_restoreContext = false;
				if(SynchronizationContext.Current == t_wfContext)
					SynchronizationContext.SetSynchronizationContext(_prevContext);
			}
			if(_prevAutoInstall) WindowsFormsSynchronizationContext.AutoInstall = true;
		}
#if DEBUG
		bool _disposed;
		~EnsureWindowsFormsSynchronizationContext_() { Debug.Assert(_disposed); }
#endif

		/// <summary>
		/// If synchronization context of this thread is null or not WindowsFormsSynchronizationContext, makes it WindowsFormsSynchronizationContext.
		/// Use this instead of creating instance when will not need to restore previous synchronization context.
		/// </summary>
		/// <exception cref="InvalidOperationException">This thread has a synchronization context other than WindowsFormsSynchronizationContext or null. Or it is null and thread's GetApartmentState is not STA.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static WindowsFormsSynchronizationContext EnsurePermanently()
		{
			var c = SynchronizationContext.Current;
			if(!(c is WindowsFormsSynchronizationContext wfc)) {
				if(!(c == null && Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)) {
					Debug.Assert(false);
					throw new InvalidOperationException("this thread has wrong SynchronizationContext type or is not STA");
				}
				t_wfContext ??= new WindowsFormsSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(t_wfContext);
				WindowsFormsSynchronizationContext.AutoInstall = false; //prevent Application.Run/DoEvents setting wrong context
				wfc = t_wfContext;
			}
			return wfc;
		}
	}
}
