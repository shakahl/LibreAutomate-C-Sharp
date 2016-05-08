using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.IO;
//using System.Windows.Forms;
using System.Runtime.Serialization;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	public class CatkeysException :Exception
	{
		const string _m = "Failed.";

		public CatkeysException() : base(_m) { }

		public CatkeysException(string message) : base(message) { }

		public CatkeysException(string message, Exception innerException) : base(message ?? _m, innerException) { }

		protected CatkeysException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	public class WaitTimeoutException :CatkeysException
	{
		const string _m = "Wait timeout.";

		public WaitTimeoutException() : base(_m) { }

		public WaitTimeoutException(string message) : base(message) { }

		public WaitTimeoutException(string message, Exception innerException) : base(message ?? _m, innerException) { }

		protected WaitTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// An alternative (to exceptions) way of indicating that a function failed.
	/// </summary>
	[DebuggerStepThrough]
	public static class ThreadError
	{
		struct _ThreadError
		{
			int _winError;
			string _catkeysError;
			Exception _exc;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Clear()
			{
				_winError = 0;
				_catkeysError = null;
				_exc = null;
				Api.SetLastError(0);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Set(int winErrorCode, string errorText)
			{
				_winError = winErrorCode;
				_catkeysError = errorText;
				_exc = null;
			}

			internal void SetException(Exception e)
			{
				_winError = 0;
				_catkeysError = null;
				_exc = e;
			}

			internal Exception GetException()
			{
				if(_exc == null) _exc = CreateException(_winError, _catkeysError);
				return _exc;
			}

			internal static Exception CreateException(int winErrorCode, string catkeysError)
			{
				if(catkeysError != null) {
					return new CatkeysException(CreateErrorString(winErrorCode, catkeysError));
				}
				if(winErrorCode != 0) {
					return new System.ComponentModel.Win32Exception(winErrorCode);
				}
				return null;
			}

			internal string GetErrorText()
			{
				if(_exc != null) return _exc.Message;
				return CreateErrorString(_winError, _catkeysError);
			}

			internal static string CreateErrorString(int winErrorCode, string errorText)
			{
				if(winErrorCode != 0) {
					string s = new System.ComponentModel.Win32Exception(winErrorCode).Message;
					if(errorText == null) return s;
					return errorText + "  " + s + ".";
				}
				return errorText;
			}

			internal int GetWinErrorCode() { return _winError; }

			internal bool IsError() { return _winError != 0 || _catkeysError != null || _exc!=null; }
		}

		[ThreadStatic]
		static _ThreadError _threadError;

		/// <summary>
		/// Sets "no error".
		/// Sets internal variables to 0/null and calls Api.SetLastError(0).
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Clear()
		{
			_threadError.Clear();
		}

		/// <summary>
		/// Calls Set(Marshal.GetLastWin32Error()).
		/// Always returns false, so you can use code like this: <c>return Api.Function(...) || ThreadError.SetWinError();</c>.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetWinError()
		{
			_threadError.Set(Marshal.GetLastWin32Error(), null);
			return false;
		}

		/// <summary>
		/// Calls Marshal.GetLastWin32Error() and returns its return value. Sets thread error if it returns not 0.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static int SetIfWinError()
		{
			int e = Marshal.GetLastWin32Error();
			if(e != 0) _threadError.Set(e, null);
			return e;
		}

		/// <summary>
		/// Sets thread error.
		/// Always returns false.
		/// </summary>
		/// <param name="winErrorCode">A Windows error code, such as those retrieved by Marshal.GetLastWin32Error() or returned by some Windows API functions. If 0, gets last Windows error (Marshal.GetLastWin32Error()); if it returns 0, uses Api.E_FAIL.</param>
		/// <param name="errorText">Error text.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool Set(int winErrorCode, string errorText = null)
		{
			if(winErrorCode == 0) winErrorCode = _GetDefaultWinError();
			_threadError.Set(winErrorCode, errorText);
			return false;
		}

		/// <summary>
		/// Sets thread error.
		/// Always returns false.
		/// </summary>
		/// <param name="errorText">Error text.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool Set(string errorText)
		{
			_threadError.Set(0, errorText);
			return false;
		}

		/// <summary>
		/// Sets thread error as Exception object.
		/// Always returns false.
		/// </summary>
		/// <param name="e">Exception.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetException(Exception e)
		{
			_threadError.SetException(e);
			return false;
		}

		static int _GetDefaultWinError()
		{
			int winErr = Marshal.GetLastWin32Error();
			return winErr != 0 ? winErr : Api.E_FAIL;
		}

		/// <summary>
		/// Sets thread error (calls Set()) or throws exception, depending on the 'throwException' argument.
		/// Always returns false.
		/// </summary>
		/// <param name="winErrorCode">A Windows error code, such as those retrieved by Marshal.GetLastWin32Error() or returned by some Windows API functions. If 0, gets last Windows error (Marshal.GetLastWin32Error()); if it returns 0, uses Api.E_FAIL.</param>
		/// <param name="errorText">Error text.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool ThrowOrSet(bool throwException, int winErrorCode = 0, string errorText = null)
		{
			if(winErrorCode == 0) winErrorCode = _GetDefaultWinError();
			if(throwException) throw _ThreadError.CreateException(winErrorCode, errorText);
			_threadError.Set(winErrorCode, errorText);
			return false;
		}

		/// <summary>
		/// Sets thread error (calls Set()) or throws exception, depending on the 'throwException' argument.
		/// Always returns false.
		/// </summary>
		/// <param name="errorText">Error text. If null, uses "Failed."</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool ThrowOrSet(bool throwException, string errorText)
		{
			if(errorText == null) errorText = "Failed.";
			if(throwException) throw _ThreadError.CreateException(0, errorText);
			_threadError.Set(0, errorText);
			return false;
		}

		/// <summary>
		/// Throws exception (see Get()) if there was error.
		/// Else returns false.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool ThrowIfError()
		{
			Exception e = _threadError.GetException();
			if(e != null) throw e;
			return false;
		}

		/// <summary>
		/// Returns true if there was error.
		/// </summary>
		public static bool IsError { get { return _threadError.IsError(); } }

		/// <summary>
		/// Gets Windows error code (see Set()).
		/// Returns 0 if Windows error code wasn't set (although a string or Exception error may be set).
		/// </summary>
		public static int WinErrorCode { get { return _threadError.GetWinErrorCode(); } }

		/// <summary>
		/// Gets error text.
		/// It can be error text as set by Set(), or Windows error description, or both, or Message of Exception object.
		/// Returns null if there was no error.
		/// </summary>
		public static string ErrorText { get { return _threadError.GetErrorText(); } }

		/// <summary>
		/// If there was error, creates and returns an exception object (CatkeysException or System.ComponentModel.Win32Exception).
		/// Else returns null.
		/// </summary>
		public static Exception Exception { get { return _threadError.GetException(); } }
	}
}
