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
	/// Can be used by tool dialogs to display common info in <see cref="InfoBox"/> control.
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
				wild = wild[10..];
			}
			_SetInfoText(text + wild);
		}

		void _SetInfoText(string text)
		{
			_control.Z.SetText(text);
		}

		void _Regex(string _)
		{
			_regexWindow ??= new RegexWindow();
			if (_regexWindow.Hwnd.Is0) {
				//TODO: now it is a hybrid of winforms (_control) and WPF (_regexWindow). Eg toolwindow links don't insert text in the focused winforms control.
				_regexWindow.ShowByRect(_control.Hwnd().Window, System.Windows.Controls.Dock.Bottom);
			} else _regexWindow.Hwnd.ShowLL(true);
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
