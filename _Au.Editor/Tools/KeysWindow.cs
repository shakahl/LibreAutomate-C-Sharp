﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using Au.Types;
using static Au.AStatic;
using Au.Controls;

namespace Au.Tools
{
	class KeysWindow : InfoWindow
	{
		public KeysWindow()
		{
			this.Size = Util.ADpi.ScaleSize((500, 250));
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
			this.Text = s;
		}

		/// <summary>
		/// A text control in which to insert the link text when clicked.
		/// If null, uses the focused control.
		/// </summary>
		public Control InsertInControl { get; set; }

		void _Insert(string s)
		{
			var sci = InsertInControl as AuScintilla;
			var pos8 = sci.Z.CurrentPos8;

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
					TUtil.InsertTextInControl(sci, prefix + s + suffix);
				};
				var p = AMouse.XY;
				m.Show(new Rectangle(p.x, p.y, 0, 0));
			} else {
				s = prefix + s + suffix;
				TUtil.InsertTextInControl(sci, s);
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
	}
}
