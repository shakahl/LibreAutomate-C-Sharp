using System;

using Au.Types;
using Au.Controls;
using Au.Util;

namespace Au.Tools
{
	class KeysWindow : InfoWindow //KPopup
	{
		public KeysWindow() : base(0) {
			Size = (500, 240);
			WindowName = "Keys";
			Name = "Ci.Keys"; //prevent hiding when activated
			CloseHides = true;
		}

		protected override void OnHandleCreated() {
			var c = Control1;
			c.ZTags.AddLinkTag("+a", o => _Insert(o)); //link that inserts a key etc
			c.ZTags.SetLinkStyle(new SciTags.UserDefinedStyle { textColor = 0x0080FF, underline = false }); //remove underline from links

			var s = AResources.GetString("tools/keys.txt").RegexReplace(@"\{(.+?)\}(?!\})", "<+a>$1<>");
			this.Text = s;

			base.OnHandleCreated();
		}

		void _Insert(string s) {
			var sci = InsertInControl as KScintilla;
			var pos8 = sci.Z.CurrentPos8;

			switch (s) {
			case "text": _AddArg(sci, pos8, ", \"!\b\""); return;
			case "html": _AddArg(sci, pos8, ", \"%\b\""); return;
			case "sleepMs": _AddArg(sci, pos8, ", 100"); return;
			case "keyCode": _AddArg(sci, pos8, ", KKey.Left"); return;
			case "scanCode": _AddArg(sci, pos8, ", new KKeyScan(1, false)"); return;
			case "action": _AddArg(sci, pos8, ", new Action(() => { AMouse.RightClick(); })"); return;
			}

			static void _AddArg(KScintilla sci, int pos8, string s) {
				pos8 = sci.Z.FindText(false, "\"", pos8) + 1; if (pos8 == 0) return;
				sci.Z.GoToPos(false, pos8);
				InsertCode.TextSimplyInControl(sci, s);
			}

			if (s.Length == 2 && s[0] != '#' && !AChar.IsAsciiAlpha(s[0])) s = s[0] == '\\' ? "|" : s[..1]; //eg 2@ or /? or \|

			char _CharAt(int pos) => (char)sci.Call(Sci.SCI_GETCHARAT, pos);

			string prefix = null, suffix = null;
			char k = _CharAt(pos8 - 1), k2 = _CharAt(pos8);
			if (s[0] == '*' || s[0] == '+') {
				if (k == '*' || k == '+') sci.Z.Select(false, pos8 - 1, pos8); //eg remove + from Alt+ if now selected *down
			} else {
				if (k > ' ' && k != '\"' && k != '(' && k != '$' && !(k == '+' && _CharAt(pos8 - 2) != '#')) prefix = " ";
			}
			if (0 != s.Ends(false, "Alt", "Ctrl", "Shift", "Win")) suffix = "+";
			else if (k2 > ' ' && k2 != '\"' && k2 != ')' && k2 != '+' && k2 != '*') suffix = " ";

			bool ok = true;
			if (s.Starts("right")) ok = _Menu("RAlt", "RCtrl", "RShift", "RWin");
			else if (s.Starts("lock")) ok = _Menu("CapsLock", "NumLock", "ScrollLock");
			else if (s.Starts("other")) ok = _Menu(s_rare);
			if (!ok) return;

			bool _Menu(params string[] a) {
				int j = AMenu.ShowSimple(a, Hwnd) - 1;
				if (j < 0) return false;
				s = a[j];
				j = s.IndexOf(' '); if (j > 0) s = s[..j];
				return true;
			}

			s = prefix + s + suffix;
			InsertCode.TextSimplyInControl(sci, s);
			if (suffix == " ") sci.Call(Sci.SCI_CHARLEFT);
		}

		static string[] s_rare = {
"BrowserBack", "BrowserForward", "BrowserRefresh", "BrowserStop", "BrowserSearch", "BrowserFavorites", "BrowserHome",
"LaunchMail", "LaunchMediaSelect", "LaunchApp1", "LaunchApp2",
"MediaNextTrack", "MediaPrevTrack", "MediaStop", "MediaPlayPause",
"VolumeMute", "VolumeDown", "VolumeUp",
"IMEKanaMode", "IMEHangulMode", "IMEJunjaMode", "IMEFinalMode", "IMEHanjaMode", "IMEKanjiMode", "IMEConvert", "IMENonconvert", "IMEAccept", "IMEModeChange", "IMEProcessKey",
"Break  //Ctrl+Pause", "Clear  //Shift+#5", "Sleep",
//"F13", "F14", "F15", "F16", "F17", "F18", "F19", "F20", "F21", "F22", "F23", "F24", //rejected
  };
	}
}
