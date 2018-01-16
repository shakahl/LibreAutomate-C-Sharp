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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// PCRE regular expressions.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe class Regex_
	{
		LPARAM _code, _md;

		///
		~Regex_()
		{
			int size = Cpp.Cpp_RegexSize(this);
			Cpp.Cpp_RegexDtor(this);
			GC.RemoveMemoryPressure(size);
		}

		/// <summary>
		/// Compiles regular expression string.
		/// </summary>
		/// <param name="rx"></param>
		/// <param name="flags"></param>
		public Regex_(string rx, RXFlags flags = RXFlags.UTF)
		{
			if(!Cpp.Cpp_RegexParse(this, rx, rx.Length, flags, out var errStr)) {
				throw new ArgumentException(errStr);
			}
			int size = Cpp.Cpp_RegexSize(this);
			GC.AddMemoryPressure(size);
		}

		/// <summary>
		/// Returns true if string s matches this regular expression.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="flags"></param>
		public bool Match(string s, RMFlags flags = 0)
		{
			if(s == null) return false;
			return Cpp.Cpp_RegexMatch(this, s, s.Length, 0, flags);
		}

		/// <summary>
		/// Returns true if the specified part of string s matches this regular expression.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="start">Start index of the s part.</param>
		/// <param name="length">Length of the s part.</param>
		/// <param name="flags"></param>
		public bool Match(string s, int start, int length=-1, RMFlags flags = 0)
		{
			if(s == null) return false;
			if(length < 0) length = s.Length - start;
			if((uint)start > s.Length || (uint)start + length > s.Length) throw new ArgumentOutOfRangeException();
			return Cpp.Cpp_RegexMatch(this, s, length, start, flags);
		}

	}
}
