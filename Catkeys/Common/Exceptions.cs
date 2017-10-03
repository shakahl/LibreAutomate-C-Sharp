using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Drawing;
//using System.Linq;
using System.Runtime.Serialization;

using static Catkeys.NoClass;

namespace Catkeys.Types
{
	/// <summary>
	/// The base exception used in this library.
	/// Some constructors support Windows API error code. Then Message will contain its error description.
	/// If the string passed to the constructor starts with "*", replaces the "*" with "Failed to ". If does not end with ".", appends ".".
	/// </summary>
	public class CatException :Exception
	{
		/// <summary>
		/// Sets Message = "Failed.".
		/// Sets NativeErrorCode = 0.
		/// </summary>
		public CatException() : base("Failed.") { }

		/// <summary>
		/// Sets Message = message.
		/// Sets NativeErrorCode = 0.
		/// </summary>
		public CatException(string message) : base(message ?? "Failed.") { }

		/// <summary>
		/// Sets Message = "Failed. " + Native.GetErrorMessage(winApiErrorCode).
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : Native.GetError().
		/// </summary>
		public CatException(int winApiErrorCode) : this(winApiErrorCode, "Failed.") { }

		/// <summary>
		/// Sets Message = message + " " + Native.GetErrorMessage(winApiErrorCode).
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : Native.GetError().
		/// </summary>
		public CatException(int winApiErrorCode, string message) : base(message ?? "Failed.")
		{
			NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : Native.GetError();
		}

		/// <summary>
		/// Sets Message = message + "\r\n\t" + innerException.Message.
		/// Sets NativeErrorCode = 0.
		/// </summary>
		public CatException(string message, Exception innerException) : base(message ?? "Failed.", innerException) { }

		/// <summary>
		/// Sets Message = message + " " + Native.GetErrorMessage(winApiErrorCode) + "\r\n\t" + innerException.Message.
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : Native.GetError().
		/// </summary>
		public CatException(int winApiErrorCode, string message, Exception innerException) : base(message ?? "Failed.", innerException)
		{
			NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : Native.GetError();
		}

		/// <summary> Gets the Windows API error code. </summary>
		public int NativeErrorCode { get; protected set; }

		/// <summary> Gets error message. </summary>
		public override string Message { get => FormattedMessage ?? FormatMessage(); }

		/// <summary> String created by FormatMessage(), which should be called by the Message override if null. Initially null. </summary>
		protected string FormattedMessage;

		/// <summary>
		/// Formats error message. Sets and returns FormattedMessage.
		/// As base text, uses the text passed to the constructor (default "Failed.").
		/// If it starts with "*", replaces the "*" with "Failed to ".
		/// If it ends with "*", replaces the "*" with commonPostfix if it is not empty.
		/// If then the message does not end with ".", appends ".".
		/// If appendMessage is null, uses Native.GetErrorMessage(NativeErrorCode) if NativeErrorCode not 0.
		/// If then appendMessage is not empty, appends " " and appendMessage.
		/// Also appends InnerException.Message in new tab-indented line if InnerException is not null.
		/// </summary>
		protected string FormatMessage(string appendMessage = null, string commonPostfix = null)
		{
			var m = base.Message;

			if(!Empty(m)) {
				if(m[0] == '*') m = "Failed to " + m.Substring(1);
				if(!Empty(commonPostfix)) {
					int k = m.Length - 1;
					if(m[k] == '*') m = m.Substring(0, k) + commonPostfix;
				}
				if(!m.EndsWith_(".")) m = m + ".";
			}

			if(appendMessage == null && NativeErrorCode != 0) appendMessage = Native.GetErrorMessage(NativeErrorCode);

			if(!Empty(appendMessage)) m = m + " " + appendMessage;

			if(InnerException != null) m = m + "\r\n\t" + InnerException.Message;

			return FormattedMessage = m;
		}

		/// <summary>
		/// If errorCode is not 0, throws CatException that includes the code and its message.
		/// More info: <see cref="FormatMessage"/>.
		/// </summary>
		/// <param name="errorCode">Windows API error code or HRESULT.</param>
		/// <param name="message">Main message. The message of the error code will be appended to it.</param>
		public static void ThrowIfFailed(int errorCode, string message = null)
		{
			if(errorCode != 0) throw new CatException(errorCode, message);
		}
	}

	/// <summary>
	/// Exception thrown mostly by <see cref="Wnd"/> functions.
	/// </summary>
	/// <remarks>
	/// Some constructors support Windows API error code. Then Message also will contain its error description.
	/// If error code ERROR_INVALID_WINDOW_HANDLE, Message also depends on whether the window handle is 0.
	/// If parameter 'winApiErrorCode' is 0 or not used: if the window handle is invalid, uses ERROR_INVALID_WINDOW_HANDLE.
	/// If the string passed to the constructor starts with "*", replaces the "*" with "Failed to ". If ends with "*", replaces the "*" with " window.". If does not end with ".", appends ".".
	/// </remarks>
	public class WndException :CatException
	{
		const string _errStr_0Handle = "The window handle is 0. Usually it means 'window not found'.";
		const string _errStr_InvalidHandle = "Invalid window handle. Usually it means 'the window was closed'.";

		/// <summary>
		/// Sets NativeErrorCode = w.IsAlive ? 0 : ERROR_INVALID_WINDOW_HANDLE.
		/// Sets Message = "Failed.".
		/// </summary>
		public WndException(Wnd w)
			: base() { Window = w; NativeErrorCode = _Code(0, w); }

		/// <summary>
		/// Sets NativeErrorCode = w.IsAlive ? 0 : ERROR_INVALID_WINDOW_HANDLE.
		/// </summary>
		public WndException(Wnd w, string message)
			: base(message) { Window = w; NativeErrorCode = _Code(0, w); }

		/// <summary>
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : (w.IsAlive ? Native.GetError() : ERROR_INVALID_WINDOW_HANDLE).
		/// Sets Message = "Failed.".
		/// </summary>
		public WndException(Wnd w, int winApiErrorCode)
			: base(_Code(winApiErrorCode, w)) { Window = w; }

		/// <summary>
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : (w.IsAlive ? Native.GetError() : ERROR_INVALID_WINDOW_HANDLE).
		/// </summary>
		public WndException(Wnd w, int winApiErrorCode, string message)
			: base(_Code(winApiErrorCode, w), message) { Window = w; }

		/// <summary>
		/// Sets NativeErrorCode = w.IsAlive ? 0 : ERROR_INVALID_WINDOW_HANDLE.
		/// </summary>
		public WndException(Wnd w, string message, Exception innerException)
			: base(message, innerException) { Window = w; NativeErrorCode = _Code(0, w); }

		/// <summary>
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : (w.IsAlive ? Native.GetError() : ERROR_INVALID_WINDOW_HANDLE).
		/// </summary>
		public WndException(Wnd w, int winApiErrorCode, string message, Exception innerException)
			: base(_Code(winApiErrorCode, w), message, innerException) { Window = w; }

		static int _Code(int code, Wnd w)
		{
			if(code != 0 || w.IsAlive) return code;
			return Api.ERROR_INVALID_WINDOW_HANDLE;
		}

		/// <summary> Gets the window passed to the constructor. </summary>
		public Wnd Window { get; }

		/// <summary> Gets error message. </summary>
		public override string Message
		{
			get
			{
				if(FormattedMessage == null) {
					string m;
					if(Window.Is0) m = _errStr_0Handle;
					else if(NativeErrorCode == Api.ERROR_INVALID_WINDOW_HANDLE) m = _errStr_InvalidHandle;
					else m = null; //will append Native.GetErrorMessage(NativeErrorCode) if NativeErrorCode not 0, or InnerException.Message if it is not null.
					FormatMessage(m, " window.");
				}
				return FormattedMessage;
			}
		}
	}

	/// <summary>
	/// Functions that search for an object can throw this exception when not found.
	/// </summary>
	public class NotFoundException :Exception
	{
		/// <summary>
		/// Sets Message = "Not found.".
		/// </summary>
		public NotFoundException() : base("Not found.") { }

		/// <summary>
		/// Sets Message = message.
		/// </summary>
		public NotFoundException(string message) : base(message) { }
	}

	public static partial class ExtensionMethods
	{
		/// <summary>
		/// If this is null, throws <see cref="NotFoundException"/>, else returns this.
		/// Used with 'find object' or 'get object' functions that return null if not found.
		/// Example: <c>var a = Acc.Find(...).OrThrow();</c>. It is equivalent to <c>var a = Acc.Find(...) ?? throw new NotFoundException();</c>.
		/// This extension method is added to classes that implement <see cref="ISupportOrThrow"/> interface.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static T OrThrow<T>(this T t) where T : class, ISupportOrThrow
		{
			return t ?? throw new NotFoundException();
		}

		/// <summary>
		/// If this is default(Wnd), throws <see cref="NotFoundException"/>, else returns this.
		/// Used with <see cref="Wnd.Find"/> and other functions that return default(Wnd) if not found.
		/// Example: <c>var w = Wnd.Find(...).OrThrow();</c>. It is equivalent to <c>var w = Wnd.Find(...); if(w.Is0) throw new NotFoundException();</c>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Wnd OrThrow(this Wnd t)
		{
			if(t.Is0) throw new NotFoundException();
			return t;
		}
	}

	/// <summary>
	/// Adds <see cref="ExtensionMethods.OrThrow{T}(T)"/> method to classes that implement this interface.
	/// Actually this interface has no methods to implement. Just add ISupportOrThrow to the implemented interfaces list in class declaration.
	/// Not used with struct. If you want to add OrThrow to a struct, add it as a normal or extension method of that struct. Example - <see cref="ExtensionMethods.OrThrow(Wnd)"/>.
	/// </summary>
	public interface ISupportOrThrow { }
}
