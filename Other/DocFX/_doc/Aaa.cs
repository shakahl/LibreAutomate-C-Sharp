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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;

//#if DEBUG //no, then DocFX skips this. Better set build action "None"; then DocFX skips this file.
//done 2021-02-26: review all #if, to ensure that DocFX does not skip something useful.
namespace Au
{
	/// <summary>
	/// DocFX tests.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class AaaDocFX
	{
		public AaaDocFX() { }

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
		/// <table>
		/// <tr>
		/// <th>a</th>
		/// <th>b</th>
		/// </tr>
		/// <tr>
		/// <td>a `*md*` b <c>*xml*</c> c</td>
		/// <td>*city*</td>
		/// </tr>
		/// </table>
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
		/// var s=@"\\?\"; //comment,,
		/// </code>
		/// 
		/// <code><![CDATA[
		/// var v = dialog.show("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { print.it(e.LinkHref); });
		/// ]]></code>
		/// 
		///  C# keywords
		/// <code><![CDATA[
		/// abstract as base bool break byte case catch char checked class Moo {};
		/// const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface IMoo {};
		/// internal is lock long namespace NMoo{}
		/// new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct SMoo {};
		/// switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while
		/// add alias ascending async await by descending dynamic equals from get global group into join let nameof on orderby partial remove select set value var
		/// when where yield
		/// unmanaged notnull record init nint nuint not and or
		/// ]]></code>
		/// </example>
		public int TestMeth(string s)
		{

			return 0;
		}

		/// <summary>
		/// Test property.
		/// </summary>
		/// <remarks>
		/// type <see cref="elm"/>
		/// 
		/// method <see cref="elm.find"/>
		/// 
		/// property <see cref="elm.Name"/>
		/// 
		/// enum <see cref="EFFlags.NotInProc"/>
		/// 
		/// API <msdn>GetTickCount</msdn>
		/// 
		/// link <see href="https://www.quickmacros.com">QM</see>
		/// 
		/// conceptual md [concept](xref:wildcard_expression)
		/// 
		/// conceptual see <see href="../articles/Wildcard%20expression.html">concept</see>
		/// 
		/// ctor <see cref="AaaDocFX()"/>
		/// 
		/// generic class <see cref="AaaDocFXT{T1, T2}"/>
		/// 
		/// generic method <see cref="TestMeth2{T1, T2}(string)"/>
		/// 
		/// markdown list
		/// - a `*md*` b <c>*xml*</c> c
		/// - two,,
		/// 
		/// xml list
		/// <list type="bullet">
		/// <item>one</item>
		/// <item>two</item>
		/// </list>
		/// 
		/// html list
		/// <ul>
		/// <li>a `*md*` b <c>*xml*</c> c</li>
		/// <li>two</li>
		/// </ul>
		/// 
		/// paragraph
		/// 
		/// line1
		/// line2
		/// 
		/// paragraph
		/// </remarks>
		public int TestProp => 0;

		/// <summary>SUMMARY =
		/// a <c>@"\\?\"</c> b</summary>
		/// <param name="s">PARAM = a <c>@"\\?\"</c> b</param>
		/// <returns>RETURNS = a <c>@"\\?\"</c> b</returns>
		/// <exception cref="Exception">EXCEPTION = a <c>@"\\?\"</c> b</exception>
		/// <remarks>REMARKS = a <c>@"\\?\"</c> b</remarks>
		/// <example>EXAMPLE = a <c>@"\\?\"</c> b</example>
		public int TestMeth2<T1, T2>(string s)
		{

			return 0;
		}
	}

	public class AaaDocFXT<T1, T2>
	{

		public int TestProp => 0;
	}
}

//#endif
