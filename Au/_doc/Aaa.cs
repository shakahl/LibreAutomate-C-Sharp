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

namespace Au
{
	/// <summary>
	/// Used to test DocFX modifications etc. //TODO: remove.
	/// </summary>
	/// <remarks>
	/// type <see cref="Acc"/>
	/// 
	/// method <see cref="Acc.Find"/>
	/// 
	/// property <see cref="Acc.Name"/>
	/// 
	/// enum <see cref="AFFlags.NotInProc"/>
	/// 
	/// API <msdn>GetTickCount</msdn>
	/// 
	/// link <see href="http://www.quickmacros.com">QM</see>
	/// 
	/// conceptual md [concept](xref:wildcard_expression)
	/// 
	/// conceptual see <see href="../articles/Wildcard%20expression.html">concept</see>
	/// 
	/// list
	/// - one
	/// - two
	/// 
	/// <list type="bullet">
	/// <item>one</item>
	/// <item>two</item>
	/// </list>
	/// 
	/// <ul>
	/// <li>one</li>
	/// <li>two</li>
	/// </ul>
	/// 
	/// line1  
	/// line2
	/// 
	/// line1\
	/// line2
	/// 
	/// 
	/// a `'\\'` b
	/// 
	/// a `@"\\?\"` b
	/// 
	/// a <c>'\\'</c> b
	/// 
	/// a <c>@"\\?\"</c> b
	/// 
	/// a <c>@"\\?\ "</c> b
	/// 
	/// <code>
	/// a '\\' b
	/// a @"\\?\" b
	/// </code>
	/// 
	/// ```csharp
	/// a '\\' b
	/// a @"\\?\" b
	/// ```
	/// 
	/// ```csharp
	/// Au.Func("string");
	/// ```
	/// <code><![CDATA[
	/// AuDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
	/// ]]></code>
	/// 
	/// </remarks>
	/// <example>
	///  cdata
	/// <code><![CDATA[
	/// a '\\' b
	/// a @"\\?\" b
	/// ]]></code>
	///  no cdata
	/// <code>
	/// a '\\' b
	/// a @"\\?\" b
	/// </code>
	///  no cdata 3
	/// <code>
	/// a '\\' b
	/// a @"\\?\ " b
	/// a @"c:\noo" b
	/// </code>
	/// 
	/// <code>
	/// var s = @"c:c"; //verbatim string
	/// </code>
	/// 
	/// <code>
	/// var s = @"c:\"; //verbatim string that ends with \
	/// </code>
	/// 
	/// <code>
	/// F(@"c:\", etc); //verbatim string that ends with \
	/// </code>
	/// 
	/// <code>
	/// var s = @"\\?\"; //verbatim string that ends with \
	/// </code>
	/// 
	/// <code><![CDATA[
	/// AuDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
	/// ]]></code>
	/// </example>
	public class Aaa
	{
		/// <summary>
		/// Test property.
		/// </summary>
		public int TestProp => 0;

		/// <summary>
		/// Test method.
		/// @s mmm
		/// </summary>
		/// <param name="s">Text.</param>
		public void TestMeth(string s) { }
	}
}
