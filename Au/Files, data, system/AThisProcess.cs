//#define USE_WTS

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
using System.Globalization;

using Au.Types;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Contains static functions to work with current process.
	/// </summary>
	/// <seealso cref="AProcess"/>
	/// <seealso cref="ATask"/>
	/// <seealso cref="Process"/>
	public static unsafe class AThisProcess
	{
		/// <summary>
		/// Gets current process id.
		/// See API <msdn>GetCurrentProcessId</msdn>.
		/// </summary>
		public static int Id => Api.GetCurrentProcessId();

		/// <summary>
		/// Returns current process handle.
		/// See API <msdn>GetCurrentProcess</msdn>.
		/// Don't need to close the handle.
		/// </summary>
		public static IntPtr Handle => Api.GetCurrentProcess();

		//rejected. Too simple and rare.
		///// <summary>
		///// Gets native module handle of the program file of this process.
		///// </summary>
		//public static IntPtr ExeModuleHandle => Api.GetModuleHandle(null);

		/// <summary>
		/// Gets full path of the program file of this process.
		/// </summary>
		[SkipLocalsInit]
		public static unsafe string ExePath {
			get {
				if (s_exePath == null) {
					var a = stackalloc char[500];
					int n = Api.GetModuleFileName(default, a, 500);
					s_exePath = new string(a, 0, n);
					//documented and tested: can be C:\SHORT~1\NAME~1.exe or \\?\C:\long path\name.exe.
					//tested: AppContext.BaseDirectory gets raw path, like above examples. Used by AFolders.ThisApp.
					//tested: CreateProcessW supports long paths in lpApplicationName, but my tested apps then crash.
					//tested: ShellExecuteW does not support long paths.
					//tested: Windows Explorer cannot launch exe if long path.
					//tested: When launched with path containing .\, ..\ or /, here we get normalized path.
				}
				return s_exePath;
			}
		}
		static string s_exePath, s_exeName;

		/// <summary>
		/// Gets file name of the program file of this process, like "name.exe".
		/// </summary>
		public static string ExeName => s_exeName ??= APath.GetName(ExePath);

		/// <summary>
		/// Gets drive type (fixed, removable, network, etc) of the program file of this process.
		/// </summary>
		/// <seealso cref="AFolders.ThisAppDriveBS"/>
		public static DriveType ExeDriveType => s_driveType ??= new DriveInfo(AFolders.ThisAppDriveBS).DriveType;
		static DriveType? s_driveType;

		//public static Process GetProcessObject(IntPtr processHandle)
		//{
		//	int pid = GetProcessId(processHandle);
		//	if(pid == 0) return null;
		//	return Process.GetProcessById(pid); //slow, makes much garbage, at first gets all processes just to throw exception if pid not found...
		//}

		/// <summary>
		/// Gets user session id of this process.
		/// Calls API <msdn>ProcessIdToSessionId</msdn> and <msdn>GetCurrentProcessId</msdn>.
		/// </summary>
		public static int SessionId => AProcess.GetSessionId(Api.GetCurrentProcessId());

		/// <summary>
		/// Gets or sets whether <see cref="CultureInfo.DefaultThreadCurrentCulture"/> and <see cref="CultureInfo.DefaultThreadCurrentUICulture"/> are <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <remarks>
		/// If your app doesn't want to use current culture (default in .NET apps), it can set these properties = <see cref="CultureInfo.InvariantCulture"/> or set this property = true.
		/// It prevents potential bugs when app/script/components don't specify invariant culture in string functions and 'number to/from string' functions.
		/// Also, there is a bug in 'number to/from string' functions in some .NET versions with some cultures: they use wrong minus sign, not ASII '-' which is specified in Control Panel.
		/// The default compiler sets this property = true; as well as <see cref="ATask.Setup"/>.
		/// </remarks>
		public static bool CultureIsInvariant {
			get {
				var ic = CultureInfo.InvariantCulture;
				return CultureInfo.DefaultThreadCurrentCulture == ic && CultureInfo.DefaultThreadCurrentUICulture == ic;
			}
			set {
				if (value) {
					var ic = CultureInfo.InvariantCulture;
					CultureInfo.DefaultThreadCurrentCulture = ic;
					CultureInfo.DefaultThreadCurrentUICulture = ic;
				} else {
					CultureInfo.DefaultThreadCurrentCulture = null;
					CultureInfo.DefaultThreadCurrentUICulture = null;
				}
			}
		}

		/// <summary>
		/// After afterMS milliseconds invokes GC and calls API SetProcessWorkingSetSize.
		/// </summary>
		internal static void MinimizePhysicalMemory_(int afterMS) {
			Task.Delay(afterMS).ContinueWith(_ => {
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), -1, -1);
			});
		}

		//internal static (long WorkingSet, long PageFile) GetMemoryInfo_()
		//{
		//	Api.PROCESS_MEMORY_COUNTERS m = default; m.cb = sizeof(Api.PROCESS_MEMORY_COUNTERS);
		//	Api.GetProcessMemoryInfo(ProcessHandle, ref m, m.cb);
		//	return ((long)m.WorkingSetSize, (long)m.PagefileUsage);
		//}

		/// <summary>
		/// Before this process exits, either normally or on unhandled exception.
		/// </summary>
		/// <remarks>
		/// The event handler is called on <see cref="AppDomain.ProcessExit"/> (then the parameter is null) and <see cref="AppDomain.UnhandledException"/> (then the parameter is <b>Exception</b>).
		/// </remarks>
		public static event Action<Exception> Exit {
			add {
				if (!_haveEventExit) {
					lock ("AVCyoRcQCkSl+3W8ZTi5oA") {
						if (!_haveEventExit) {
							var d = AppDomain.CurrentDomain;
							d.ProcessExit += _ProcessExit;
							d.UnhandledException += _ProcessExit; //because ProcessExit is missing on exception
							_haveEventExit = true;
						}
					}
				}
				_eventExit += value;
			}
			remove {
				_eventExit -= value;
			}
		}
		static Action<Exception> _eventExit;
		static bool _haveEventExit;

		static void _ProcessExit(object sender, EventArgs ea) //sender: AppDomain on process exit, null on unhandled exception
		{
			Exception e;
			if (ea is UnhandledExceptionEventArgs u) {
				if (!u.IsTerminating) return; //never seen, but anyway
				e = (Exception)u.ExceptionObject; //probably non-Exception object is impossible in C#
			} else {
				e = ATask.s_unhandledException;
			}
			var k = _eventExit;
			if (k != null) try { k(e); } catch { }
		}
	}
}
