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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Controls;

namespace Au.Tools
{
	/// <summary>
	/// Can be used by tool dialogs to display common info in <see cref="AuInfoBox"/> control.
	/// The control must support tags.
	/// </summary>
	internal class CommonInfos
	{
		AuInfoBox _control;

		public CommonInfos(AuInfoBox control)
		{
			_control = control;
			_control.Tags.AddLinkTag("+examples", what => _Examples(what));
		}

		/// <summary>
		/// Displays text with appended wildex info that starts with "\r\nThe text is wildcard expression".
		/// If text ends with $, wildex info starts with " is wildcard expression".
		/// </summary>
		public void SetTextWithWildexInfo(string text)
		{
			string wild = c_infoWildex;
			if(text.Ends('$')) {
				text = text.RemoveSuffix(1);
				wild = wild.Substring(10);
			}
			_SetInfoText(text + wild);
		}

		void _SetInfoText(string text)
		{
			_control.ST.SetText(text);
		}

		void _Examples(string what)
		{
			switch(what) {
			case "wildex":
				_SetInfoText("whole text\n*end\nstart*\n*middle*\ntime ??:??\n**t literal text\n**c case-sensitive text\n**tc case-sensitive literal\n**r regular expression\n**rc case-sensitive regex\n**n not this\n**m this||or this||**r or this regex||**n and not this\n**m(^^^) this^^^or this^^^or this\n\nCan be verbatim string. Examples:\n@\"C:\\Example\"\n@\"**rc regular expression\"");
				break;
			//case "regex":
			//	break;
			}
		}

		const string c_infoWildex = @"
The text is <help 0248143b-a0dd-4fa1-84f9-76831db6714a>wildcard expression<>. <+examples wildex>Examples<>.
Regular expression info: <link https://www.pcre.org/current/doc/html/pcre2pattern.html>syntax</link>, <link https://www.pcre.org/current/doc/html/pcre2syntax.html>syntax summary</link>, <link http://www.rexegg.com/>rexegg.com</link>, <link https://www.regular-expressions.info/>regular-expressions.info</link>.
Can be verbatim string. Examples: <c brown>@""C:\Example""<>,  <c brown>@""**rc regular expression""<>.";
	}
}
