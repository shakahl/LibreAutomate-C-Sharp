using System;
using System.Drawing;
using System.Windows.Forms;

using Au.Types;
using Au.Controls;

namespace Au.Tools
{
	class KeysWindow : InfoWindow
	{
		public KeysWindow()
		{
			this.Size = Util.ADpi.ScaleSize((500, 300));
			this.Caption = "Keys";
		}

		protected override void OnLoad(EventArgs e)
		{
			var c = this.Control1;
			//c.Call(Sci.SCI_SETWRAPSTARTINDENT, 4);
			//c.ZTags.AddStyleTag(".h", new SciTags.UserDefinedStyle { backColor = 0xC0E0C0, bold = true, eolFilled = true }); //topic header
			c.ZTags.AddLinkTag("+a", o => _Insert(o)); //link that inserts a key etc
			c.ZTags.SetLinkStyle(new SciTags.UserDefinedStyle { textColor = 0x0080FF, underline = false }); //remove underline from links

			var s = EdResources.GetEmbeddedResourceString("Au.Editor.Tools.Keys.txt").RegexReplace(@"\{(.+?)\}(?!\})", "<+a>$1<>");
			var z = this.Window.ClientSize; z.Height = c.Z.LineHeight() * s.LineCount() + 6; this.Window.ClientSize = z;
			this.Text = s;
		}

		void _Insert(string s)
		{
			var sci = InsertInControl;
			var pos8 = sci.Z.CurrentPos8;

			switch(s) {
			case "text": _AddParameter(sci, pos8, ", (KText)\"%\""); return;
			case "sleepMs": _AddParameter(sci, pos8, ", 100"); return;
			case "keyCode": _AddParameter(sci, pos8, ", KKey.Left"); return;
			case "scanCode": _AddParameter(sci, pos8, ", (1, false)"); return;
			case "action": _AddParameter(sci, pos8, ", new Action(() => { AMouse.RightClick(); })"); return;
			}

			if(s.Length == 2 && s[0] != '#' && !AChar.IsAsciiAlpha(s[0])) s = s[0] == '\\' ? "|" : s[..1]; //eg 2@ or /? or \|

			char _CharAt(int pos) => (char)sci.Call(Sci.SCI_GETCHARAT, pos);

			string prefix = null, suffix = null;
			char k = _CharAt(pos8 - 1), k2 = _CharAt(pos8);
			if(s[0] == '*' || s[0] == '+') {
				if(k == '*' || k == '+') sci.Z.Select(false, pos8 - 1, pos8); //eg remove + from Alt+ if now selected *down
			} else {
				if(k > ' ' && k != '\"' && k != '(' && k != '$' && !(k == '+' && _CharAt(pos8 - 2) != '#')) prefix = " ";
			}
			if(0 != s.Ends(false, "Alt", "Ctrl", "Shift", "Win")) suffix = "+";
			else if(k2 > ' ' && k2 != '\"' && k2 != ')' && k2 != '+' && k2 != '*') suffix = " ";

			if(s == "name") {
				//var a = typeof(KKey).GetEnumNames().Where(o => !(o.Length==1 ||(o.Length==2&& o[0]=='D') || o.Starts("Mouse"))).ToArray();
				//Array.Sort(a);
				var m = new PopupList { Items = s_keys };
				m.SelectedAction = m => {
					var s = m.ResultItem as string;
					int i = s.IndexOf(' '); if(i > 0) s = s[0..i];
					InsertCode.TextSimplyInControl(sci, prefix + s + suffix);
				};
				var p = AMouse.XY;
				m.Show(new Rectangle(p.x, p.y, 0, 0));
			} else {
				s = prefix + s + suffix;
				InsertCode.TextSimplyInControl(sci, s);
			}

			if(suffix == " ") sci.Call(Sci.SCI_CHARLEFT);
		}

		static string[] s_keys = {
"BrowserBack", "BrowserForward", "BrowserRefresh", "BrowserStop", "BrowserSearch", "BrowserFavorites", "BrowserHome",
"LaunchMail", "LaunchMediaSelect", "LaunchApp1", "LaunchApp2",
"MediaNextTrack", "MediaPrevTrack", "MediaStop", "MediaPlayPause",
"VolumeMute", "VolumeDown", "VolumeUp",
"IMEKanaMode", "IMEHangulMode", "IMEJunjaMode", "IMEFinalMode", "IMEHanjaMode", "IMEKanjiMode", "IMEConvert", "IMENonconvert", "IMEAccept", "IMEModeChange", "IMEProcessKey",
"Break  //Ctrl+Pause", "Clear  //Shift+#5", "Sleep",
"F13", "F14", "F15", "F16", "F17", "F18", "F19", "F20", "F21", "F22", "F23", "F24",
  };

		void _AddParameter(AuScintilla sci, int pos8, string s)
		{
			pos8 = sci.Z.FindText(false, "\"", pos8) + 1; if(pos8 == 0) return;
			sci.Z.GoToPos(false, pos8);
			InsertCode.TextSimplyInControl(sci, s);
		}
	}
}
