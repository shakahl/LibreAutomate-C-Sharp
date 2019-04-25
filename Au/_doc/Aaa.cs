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
	/// a <c>int i\=2 \*italic\* &quot;s&quot;; \/\/comm</c> b
	/// a <c>\*italic\*</c> b
	/// a <c>"string"</c> b
	/// a <c>\@"\\\\\?\\"</c> b
	/// a <c>\!"\#\$%'\(\)\*\+\,\-\.\/\:;\=\?\@\[\\\]\^\_\`\{\|\}\~</c> b
	/// a <c><![CDATA[\!"\#\$%&'\(\)\*\+\,\-\.\/\:;<\=\>\?\@\[\\\]\^\_\`\{\|\}\~]]></c> b
	/// a <c>bbb\\</c> b
	/// </remarks>
	public class AaaDocFX
	{
		/// <summary>
		/// Test property.
		/// </summary>
		public int TestProp => 0;

		/// <summary>
		/// SUMMARY
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
		/// head <c>int i=2 *italic* "s"; //comm</c> tail
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
		/// var s=@"\\?\"; //comment
		/// </code>
		/// </example>
		public int TestMeth(string s)
		{
			if(!Keyb.WaitForHotkey(-3, "Ctrl+Shift+K")) return 0;
			return 0;
		}
	}
}
//#endif
