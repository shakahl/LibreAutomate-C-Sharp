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

using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Manages a kernel handle.
	/// Must be disposed.
	/// Has static functions to open process handle.
	/// </summary>
	internal struct LibKernelHandle : IDisposable
	{
		IntPtr _h;

		/// <summary>
		/// Attaches a kernel handle to this new variable.
		/// No exception when handle is invalid.
		/// </summary>
		/// <param name="handle"></param>
		public LibKernelHandle(IntPtr handle) { _h = handle; }

		public static explicit operator LibKernelHandle(IntPtr p) => new LibKernelHandle(p);

		public static implicit operator IntPtr(LibKernelHandle p) => p._h;

		///
		public IntPtr Handle => _h;

		/// <summary>_h == 0</summary>
		public bool Is0 => _h == default;

		/// <summary>_h == 0 || _h == -1</summary>
		public bool IsInvalid => _h == default || _h.ToInt64() == -1;

		///
		public void Dispose()
		{
			if(!IsInvalid) Api.CloseHandle(_h);
			_h = default;
		}

		/// <summary>
		/// Opens process handle.
		/// Calls API OpenProcess.
		/// Returns default if fails. Supports <see cref="WinError"/>.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
		public static LibKernelHandle OpenProcess(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		{
			if(processId == 0) { WinError.Code = Api.ERROR_INVALID_PARAMETER; return default; }
			_OpenProcess(out var h, processId, desiredAccess);
			return new LibKernelHandle(h);
		}

		/// <summary>
		/// Opens window's process handle.
		/// This overload is more powerful: if API OpenProcess fails, it tries API GetProcessHandleFromHwnd, which can open higher integrity level processes, but only if current process is uiAccess and desiredAccess includes only PROCESS_DUP_HANDLE, PROCESS_VM_OPERATION, PROCESS_VM_READ, PROCESS_VM_WRITE, SYNCHRONIZE.
		/// Returns default if fails. Supports <see cref="WinError"/>.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
		public static LibKernelHandle OpenProcess(Wnd w, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		{
			int pid = w.ProcessId; if(pid == 0) return default;
			_OpenProcess(out var h, pid, desiredAccess, w);
			return new LibKernelHandle(h);
		}

		static bool _OpenProcess(out IntPtr R, int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION, Wnd processWindow = default)
		{
			R = Api.OpenProcess(desiredAccess, false, processId);
			if(R != default) return true;
			if(processWindow.Is0) return false;
			if(0 != (desiredAccess & ~(Api.PROCESS_DUP_HANDLE | Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE | Api.SYNCHRONIZE))) return false;
			int e = WinError.Code;
			if(Uac.OfThisProcess.IsUIAccess) R = Api.GetProcessHandleFromHwnd(processWindow);
			if(R != default) return true;
			Api.SetLastError(e);
			return false;
		}
	}

	/// <summary>
	/// Kernel handle that is derived from WaitHandle.
	/// When don't need to wait, use <see cref="LibKernelHandle"/>, it's more lightweight and has more creation methods.
	/// </summary>
	internal class LibKernelWaitHandle : WaitHandle
	{
		public LibKernelWaitHandle(IntPtr nativeHandle, bool ownsHandle)
		{
			base.SafeWaitHandle = new SafeWaitHandle(nativeHandle, ownsHandle);
		}

		/// <summary>
		/// Opens process handle.
		/// Returns null if failed.
		/// </summary>
		/// <param name="pid"></param>
		/// <param name="desiredAccess"></param>
		public static LibKernelWaitHandle FromProcessId(int pid, uint desiredAccess)
		{
			LibKernelWaitHandle wh = null;
			try { wh = new LibKernelWaitHandle(LibKernelHandle.OpenProcess(pid, desiredAccess), true); }
			catch(Exception ex) { Debug_.Print(ex); }
			return wh;
		}
	}
}
