using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

//TODO: remove.
//#if DEBUG //no, then DocFX skips this. TODO: find all #if, to ensure that DocFX does not skip something useful...
namespace Au
{
	/// <summary>
	/// DocFX tests.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class AaaDocFX
	{
		/// <summary>
		/// Test property.
		/// </summary>
		public int TestProp => 0;

		/// <summary>
		/// SUMMARY
		/// a <c>@"\\?\"</c> b
		/// </summary>
		/// <param name="s">
		/// PARAM
		/// a <c>@"\\?\"</c> b
		/// </param>
		/// <returns>
		/// RETURNS
		/// a <c>@"\\?\"</c> b
		/// </returns>
		/// <exception cref="Exception">
		/// EXCEPTION
		/// a <c>@"\\?\"</c> b
		/// </exception>
		/// <remarks>
		/// REMARKS
		/// 
		/// a <c>@"\\network\share"
		/// line2\
		/// line3</c> b
		/// 
		/// head <c>int i=2 *italic* "s"; //comm</c> tail,
		/// 
		/// a <c>*italic*</c> b
		/// a <c>"string"</c> b
		/// a <c>@"\\?\"</c> b
		/// 
		/// a <c>"line1
		/// line2	tab"</c> b
		/// 
		/// a <c>!"#$%'()*+,-./:;=?@[\]^_`{|}~</c> b
		/// a <c><![CDATA[!"#$%&'()*+,-./:;<=>?@[\]^_`{|}~]]></c> b
		/// 
		/// a <c>&lt;&gt;&amp;&apos;&quot;</c>
		/// 
		/// a <c>bbb\</c> footer
		/// 
		/// </remarks>
		/// <example>
		/// EXAMPLE
		/// a <c>@"\\?\"</c> b
		/// <code>
		/// var c = '\\'; //comment
		/// var s=@"\\?\"; //comment,,,,
		/// </code>
		/// </example>
		public int TestMeth(string s)
		{
			
			return 0;
		}

		/// <summary>SUMMARY =
		/// a <c>@"\\?\"</c> b</summary>
		/// <param name="s">PARAM = a <c>@"\\?\"</c> b</param>
		/// <returns>RETURNS = a <c>@"\\?\"</c> b</returns>
		/// <exception cref="Exception">EXCEPTION = a <c>@"\\?\"</c> b</exception>
		/// <remarks>REMARKS = a <c>@"\\?\"</c> b</remarks>
		/// <example>EXAMPLE = a <c>@"\\?\"</c> b</example>
		public int TestMeth2(string s)
		{
			
			return 0;
		}

		/// <summary>
		/// Allows to *split* a <b>KHotkey</b> variable like <c>var (mod, key) = hotkey;</c> *italic* text. a <c>@"\\?\"</c> b
		/// </summary>
		public void Deconstruct(out KMod mod, out KKey key) { mod = 0; key = 0; }
	}
}
//#endif
