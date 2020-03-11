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
//using System.Linq;

using Au.Types;

namespace Au
{
	/// <summary>
	/// Writes warnings to the output.
	/// </summary>
	/// <seealso cref="OptWarnings"/>
	public static class AWarning
	{
		/// <summary>
		/// Writes warning text to the output.
		/// By default appends the stack trace.
		/// </summary>
		/// <param name="text">Warning text.</param>
		/// <param name="showStackFromThisFrame">If &gt;= 0, appends the stack trace, skipping this number of frames. Default 0.</param>
		/// <param name="prefix">Text before <i>text</i>. Default <c>"&lt;&gt;Warning: "</c>.</param>
		/// <remarks>
		/// Calls <see cref="AOutput.Write"/>.
		/// Does not show more than 1 warning/second, unless <b>AOpt.Warnings.Verbose</b> == true (see <see cref="OptWarnings.Verbose"/>).
		/// To disable some warnings, use code <c>AOpt.Warnings.Disable("warning text wildcard");</c> (see <see cref="OptWarnings.Disable"/>).
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Write(string text, int showStackFromThisFrame = 0, string prefix = "<>Warning: ")
		{
			if(AOpt.Warnings.IsDisabled(text)) return;

			if(!AOpt.Warnings.Verbose) {
				var t = Api.GetTickCount64();
				if(t - s_warningTime < 1000) return;
				s_warningTime = t;
			}

			string s = text ?? "";
			if(showStackFromThisFrame >= 0) {
				var x = new StackTrace(showStackFromThisFrame + 1, true);
				var st = x.ToString(); var rn = st.Ends('\n') ? "" : "\r\n";
				s = $"{prefix}{s} <fold><\a>\r\n{st}{rn}</\a></fold>";
			} else s = prefix + s;

			AOutput.Write(s);
		}
		static long s_warningTime;
	}
}
