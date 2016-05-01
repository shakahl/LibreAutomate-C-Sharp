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

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void None()
			{
				_winError = 0;
				_catkeysError = null;
				Api.SetLastError(0);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Set(int winErrorCode, string errorText)
			{
				_winError = winErrorCode;
				_catkeysError = errorText;
			}

			internal Exception GetException()
			{
				return CreateException(_winError, _catkeysError);
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

			internal string GetErrorString()
			{
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

			internal bool IsError() { return _winError != 0 || _catkeysError != null; }
		}

		[ThreadStatic]
		static _ThreadError _threadError;

		/// <summary>
		/// Sets "no error".
		/// Sets internal variables to 0/null and calls Api.SetLastError(0).
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void None()
		{
			_threadError.None();
		}

		/// <summary>
		/// Calls Marshal.GetLastWin32Error(). If it returns not 0, sets thread error (else does not reset it).
		/// Always returns false, so you can use code like this: <c>ThreadError.None(); return Api.Function(...) || ThreadError.SetIfWinError();</c>.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetIfWinError()
		{
			int winErr = Marshal.GetLastWin32Error();
			if(winErr != 0) _threadError.Set(winErr, null);
			return false;
		}

		/// <summary>
		/// Calls Marshal.GetLastWin32Error(). If it returns not 0, sets thread error (calls Set()) or throws exception, depending on the 'set' argument.
		/// Always returns false.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetOrThrowIfWinError(bool set)
		{
			int winErr = Marshal.GetLastWin32Error();
			if(winErr != 0) {
				if(set) _threadError.Set(winErr, null);
				else throw _ThreadError.CreateException(winErr, null);
			}
			return false;
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
		/// Sets thread error (calls Set()) or throws exception, depending on the 'set' argument.
		/// Always returns false.
		/// </summary>
		/// <param name="winErrorCode">A Windows error code, such as those retrieved by Marshal.GetLastWin32Error() or returned by some Windows API functions. If 0, gets last Windows error (Marshal.GetLastWin32Error()); if it returns 0, uses Api.E_FAIL.</param>
		/// <param name="errorText">Error text.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetOrThrow(bool set, int winErrorCode, string errorText = null)
		{
			if(winErrorCode == 0) winErrorCode = _GetDefaultWinError();
			if(set) _threadError.Set(winErrorCode, errorText);
			else throw _ThreadError.CreateException(winErrorCode, errorText);
			return false;
		}

		static int _GetDefaultWinError()
		{
			int winErr = Marshal.GetLastWin32Error();
			return winErr != 0 ? winErr : Api.E_FAIL;
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
		/// Sets thread error (calls Set()) or throws exception, depending on the 'set' argument.
		/// Always returns false.
		/// </summary>
		/// <param name="errorText">Error text.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetOrThrow(bool set, string errorText)
		{
			if(errorText == null) errorText = "Failed.";
			if(set) _threadError.Set(0, errorText);
			else throw _ThreadError.CreateException(0, errorText);
			return false;
		}

		/// <summary>
		/// If there was error, creates and returns an exception object (CatkeysException or System.ComponentModel.Win32Exception).
		/// Else returns null.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Exception GetException()
		{
			return _threadError.GetException();
		}

		/// <summary>
		/// Returns true if there was error.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool IsError()
		{
			return _threadError.IsError();
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
		/// Gets Windows error code (see Set()).
		/// Returns 0 if Windows error code wasn't set (although a string error may be set).
		/// </summary>
		public static int WinErrorCode { get { return _threadError.GetWinErrorCode(); } }

		/// <summary>
		/// Gets error text.
		/// It can be error text as set by Set(), or Windows error description, or both.
		/// Returns null if there was no error (neither error text nor Windows error code weren't set).
		/// </summary>
		public static string ErrorString { get { return _threadError.GetErrorString(); } }
	}
}
