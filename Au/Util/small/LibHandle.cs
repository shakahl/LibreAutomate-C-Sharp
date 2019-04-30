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
using System.Linq;
using Microsoft.Win32.SafeHandles;

using static Au.NoClass;

namespace Au.Types
{
	/// <summary>
	/// Manages a kernel handle.
	/// Must be disposed.
	/// Has static functions to open process handle.
	/// </summary>
	internal struct LibHandle : IDisposable
	{
		IntPtr _h;

		/// <summary>
		/// Attaches a kernel handle to this new variable.
		/// No exception when handle is invalid.
		/// If handle == -1, sets 0.
		/// </summary>
		/// <param name="handle"></param>
		public LibHandle(IntPtr handle) { _h = handle.ToInt64() == -1 ? default : handle; }

		//public static explicit operator LibHandle(IntPtr p) => new LibHandle(p);

		public static implicit operator IntPtr(LibHandle p) => p._h;

		/// <summary>
		/// _h == default.
		/// Info: _h never is -1.
		/// </summary>
		public bool Is0 => _h == default;

		///
		public void Dispose()
		{
			if(!Is0) { Api.CloseHandle(_h); _h = default; }
		}

		/// <summary>
		/// Opens process handle.
		/// Calls API OpenProcess.
		/// Returns default if fails. Supports <see cref="WinError"/>.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
		public static LibHandle OpenProcess(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		{
			if(processId == 0) { WinError.Code = Api.ERROR_INVALID_PARAMETER; return default; }
			return _OpenProcess(processId, desiredAccess);
		}

		/// <summary>
		/// Opens window's process handle.
		/// This overload is more powerful: if API OpenProcess fails, it tries API GetProcessHandleFromHwnd, which can open higher integrity level processes, but only if current process is uiAccess and desiredAccess includes only PROCESS_DUP_HANDLE, PROCESS_VM_OPERATION, PROCESS_VM_READ, PROCESS_VM_WRITE, SYNCHRONIZE.
		/// Returns default if fails. Supports <see cref="WinError"/>.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
		public static LibHandle OpenProcess(Wnd w, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		{
			int pid = w.ProcessId; if(pid == 0) return default;
			return _OpenProcess(pid, desiredAccess, w);
		}

		static LibHandle _OpenProcess(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION, Wnd processWindow = default)
		{
			LibHandle R = Api.OpenProcess(desiredAccess, false, processId);
			if(R.Is0 && !processWindow.Is0 && 0 == (desiredAccess & ~(Api.PROCESS_DUP_HANDLE | Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE | Api.SYNCHRONIZE))) {
				int e = WinError.Code;
				if(Uac.OfThisProcess.IsUIAccess) R = Api.GetProcessHandleFromHwnd(processWindow);
				if(R.Is0) Api.SetLastError(e);
			}
			return R;
		}
	}

	/// <summary>
	/// Kernel handle that is derived from WaitHandle.
	/// When don't need to wait, use <see cref="LibHandle"/>, it's more lightweight and has more creation methods.
	/// </summary>
	internal class LibWaitHandle : WaitHandle
	{
		public LibWaitHandle(IntPtr nativeHandle, bool ownsHandle)
		{
			base.SafeWaitHandle = new SafeWaitHandle(nativeHandle, ownsHandle);
		}

		/// <summary>
		/// Opens process handle.
		/// Returns null if failed.
		/// </summary>
		/// <param name="pid"></param>
		/// <param name="desiredAccess"></param>
		public static LibWaitHandle FromProcessId(int pid, uint desiredAccess)
		{
			LibWaitHandle wh = null;
			try { wh = new LibWaitHandle(LibHandle.OpenProcess(pid, desiredAccess), true); }
			catch(Exception ex) { Debug_.Print(ex); }
			return wh;
		}
	}
}
