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
//using System.Linq;
using System.Runtime.Serialization;

using Au.Types;
using static Au.AStatic;

namespace Au.Types
{
	/// <summary>
	/// The base exception used in this library.
	/// </summary>
	/// <remarks>
	/// Some constructors support Windows API error code. Then <see cref="Message"/> will contain its error description.
	/// If the string passed to the constructor starts with "*", replaces the "*" with "Failed to ". If does not end with ".", appends ".".
	/// </remarks>
	[Serializable]
	public class AException :Exception, ISerializable
	{
		/// <summary>
		/// Sets Message = "Failed.".
		/// Sets NativeErrorCode = 0.
		/// </summary>
		public AException() : base("Failed.") { }

		/// <summary>
		/// Sets Message = message.
		/// Sets NativeErrorCode = 0.
		/// </summary>
		public AException(string message) : base(message ?? "Failed.") { }

		/// <summary>
		/// Sets Message = "Failed. " + WinError.MessageFor(winApiErrorCode).
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : WinError.Code.
		/// </summary>
		public AException(int winApiErrorCode) : this(winApiErrorCode, "Failed.") { }

		/// <summary>
		/// Sets Message = message + " " + WinError.MessageFor(winApiErrorCode).
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : WinError.Code.
		/// </summary>
		public AException(int winApiErrorCode, string message) : base(message ?? "Failed.")
		{
			NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : WinError.Code;
		}

		/// <summary>
		/// Sets Message = message + "\r\n\t" + innerException.Message.
		/// Sets NativeErrorCode = 0.
		/// </summary>
		public AException(string message, Exception innerException) : base(message ?? "Failed.", innerException) { }

		/// <summary>
		/// Sets Message = message + " " + WinError.MessageFor(winApiErrorCode) + "\r\n\t" + innerException.Message.
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : WinError.Code.
		/// </summary>
		public AException(int winApiErrorCode, string message, Exception innerException) : base(message ?? "Failed.", innerException)
		{
			NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : WinError.Code;
		}

		/// <summary> Gets the Windows API error code. </summary>
		public int NativeErrorCode { get; protected set; }

		/// <summary> Gets error message. </summary>
		public override string Message => FormattedMessage ?? FormatMessage();

		/// <summary> String created by FormatMessage(), which should be called by the Message override if null. Initially null. </summary>
		protected string FormattedMessage;

		/// <summary>
		/// Formats error message. Sets and returns FormattedMessage.
		/// As base text, uses the text passed to the constructor (default "Failed.").
		/// If it starts with "*", replaces the "*" with "Failed to ".
		/// If it ends with "*", replaces the "*" with commonPostfix if it is not empty.
		/// If then the message does not end with ".", appends ".".
		/// If appendMessage is null, uses WinError.MessageFor(NativeErrorCode) if NativeErrorCode not 0.
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
				if(!m.Ends('.')) m = m + ".";
			}

			if(appendMessage == null && NativeErrorCode != 0) appendMessage = WinError.MessageFor(NativeErrorCode);

			if(!Empty(appendMessage)) m = m + " " + appendMessage;

			if(InnerException != null) m = m + "\r\n\t" + InnerException.Message;

			return FormattedMessage = m;
		}

		/// <summary>
		/// If errorCode is not 0, throws AException that includes the code and its message.
		/// More info: <see cref="FormatMessage"/>.
		/// </summary>
		/// <param name="errorCode">Windows API error code or HRESULT.</param>
		/// <param name="message">Main message. The message of the error code will be appended to it.</param>
		public static void ThrowIfHresultNot0(int errorCode, string message = null)
		{
			if(errorCode != 0) throw new AException(errorCode, message);
		}

		/// <summary>
		/// If errorCode is less than 0, throws AException that includes the code and its message.
		/// More info: <see cref="FormatMessage"/>.
		/// </summary>
		/// <param name="errorCode">Windows API error code or HRESULT.</param>
		/// <param name="message">Main message. The message of the error code will be appended to it.</param>
		public static void ThrowIfHresultNegative(int errorCode, string message = null)
		{
			if(errorCode < 0) throw new AException(errorCode, message);
		}

		#region ISerializable

		///
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("NativeErrorCode", NativeErrorCode);
			base.GetObjectData(info, context);
		}

		///
		protected AException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			NativeErrorCode = info.GetInt32("NativeErrorCode");
		}

		#endregion
	}

	/// <summary>
	/// Exception thrown mostly by <see cref="Wnd"/> functions.
	/// </summary>
	/// <remarks>
	/// Some constructors support Windows API error code. Then Message also will contain its error description.
	/// If error code ERROR_INVALID_WINDOW_HANDLE, Message also depends on whether the window handle is 0.
	/// If parameter <i>winApiErrorCode</i> is 0 or not used: if the window handle is invalid, uses ERROR_INVALID_WINDOW_HANDLE.
	/// If the string passed to the constructor starts with "*", replaces the "*" with "Failed to ". If ends with "*", replaces the "*" with " window.". If does not end with ".", appends ".".
	/// </remarks>
	[Serializable]
	public class WndException :AException, ISerializable
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
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : (w.IsAlive ? WinError.Code : ERROR_INVALID_WINDOW_HANDLE).
		/// Sets Message = "Failed.".
		/// </summary>
		public WndException(Wnd w, int winApiErrorCode)
			: base(_Code(winApiErrorCode, w)) { Window = w; }

		/// <summary>
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : (w.IsAlive ? WinError.Code : ERROR_INVALID_WINDOW_HANDLE).
		/// </summary>
		public WndException(Wnd w, int winApiErrorCode, string message)
			: base(_Code(winApiErrorCode, w), message) { Window = w; }

		/// <summary>
		/// Sets NativeErrorCode = w.IsAlive ? 0 : ERROR_INVALID_WINDOW_HANDLE.
		/// </summary>
		public WndException(Wnd w, string message, Exception innerException)
			: base(message, innerException) { Window = w; NativeErrorCode = _Code(0, w); }

		/// <summary>
		/// Sets NativeErrorCode = (winApiErrorCode != 0) ? winApiErrorCode : (w.IsAlive ? WinError.Code : ERROR_INVALID_WINDOW_HANDLE).
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
					else m = null; //will append WinError.MessageFor(NativeErrorCode) if NativeErrorCode not 0, or InnerException.Message if it is not null.
					FormatMessage(m, " window.");
				}
				return FormattedMessage;
			}
		}

		#region ISerializable

		///
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Window", (int)Window);
			base.GetObjectData(info, context);
		}

		///
		protected WndException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Window = (Wnd)info.GetInt32("Window");
		}

		#endregion
	}

	/// <summary>
	/// Functions that search for an object can throw this exception when not found.
	/// </summary>
	[Serializable]
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

		#region ISerializable

		///
		protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		#endregion
	}
}

namespace Au
{
	/// <summary>
	/// Extension methods for types of this library.
	/// </summary>
	public static partial class AExtAu
	{
		/// <summary>
		/// If this is default(Wnd), throws <see cref="NotFoundException"/>, else returns this.
		/// </summary>
		/// <exception cref="NotFoundException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// var w = Wnd.Find("Example").OrThrow();
		/// ]]></code>
		/// </example>
		public static Wnd OrThrow(this Wnd x) => !x.Is0 ? x : throw new NotFoundException("Not found (Wnd).");

		/// <summary>
		/// If this is null, throws <see cref="NotFoundException"/>, else returns this.
		/// </summary>
		/// <exception cref="NotFoundException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// var w = Wnd.Find("Example").OrThrow();
		/// 
		/// var a1 = Acc.Find(w, "web:LINK", "Example").OrThrow();
		/// 
		/// var a2 = (Acc.Find(w, ...)?.Find(...)).OrThrow();
		/// ]]></code>
		/// </example>
		public static Acc OrThrow(this Acc x) => x ?? throw new NotFoundException("Not found (Acc).");

		/// <summary>
		/// If this is null, throws <see cref="NotFoundException"/>, else returns this.
		/// </summary>
		/// <exception cref="NotFoundException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// var w = Wnd.Find("Example").OrThrow();
		/// var wi = WinImage.Find(w, ...).OrThrow();
		/// ]]></code>
		/// </example>
		public static WinImage OrThrow(this WinImage x) => x ?? throw new NotFoundException("Not found (WinImage).");
	}

	static partial class AExtensions
	{
		/// <summary>
		/// Returns string containing exception type name and message.
		/// </summary>
		public static string ToStringWithoutStack(this Exception t)
		{
			return t.GetType().Name + ", " + t.Message;
		}
	}
}
