using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
//using System.Linq;

using Au.Types;
using Au.Controls;

namespace Au.Tools
{
	/// <summary>
	/// Can be used by tool dialogs to display common info in <see cref="InfoBoxF"/> control.
	/// </summary>
	internal class CommonInfos
	{
		InfoBoxF _control;

		public CommonInfos(InfoBoxF control)
		{
			_control = control;
			_control.ZTags.AddLinkTag("+regex", o => _Regex(o));
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
			_control.Z.SetText(text);
		}

		void _Regex(string _)
		{
			if(_regexWindow == null) _regexWindow = new RegexWindow(_control);
			if(!_regexWindow.Window.IsHandleCreated) _regexWindow.Show(_control);
			else _regexWindow.Window.Show();
		}

		RegexWindow _regexWindow;

		const string c_infoWildex = @"
The text is <help articles/Wildcard expression>wildcard expression<>.
Can be verbatim string and contain <+regex>regular expression<>, like <c brown>@""**rc regex""<>.
Examples:
whole text
*end
start*
*middle*
time ??:??
**t literal text
**c case-sensitive text
**tc case-sensitive literal
**r regular expression
**rc case-sensitive regex
**n not this
**m this||or this||**r or this regex||**n and not this
**m(^^^) this^^^or this^^^or this
";
	}
}
