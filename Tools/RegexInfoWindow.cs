using System;
using System.IO;
using System.Windows.Forms;

using Au.Types;
using static Au.AStatic;
using Au.Controls;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Au.Tools
{
	public class RegexInfoWindow : InfoWindow
	{
		public RegexInfoWindow() : base(Util.ADpi.ScaleInt(250))
		{
			this.Size = Util.ADpi.ScaleSize((800, 220));
			this.Caption = "Regex";
		}

		protected override void OnLoad(EventArgs e)
		{
			for(int i = 0; i < 2; i++) {
				var c = i == 0 ? this.Control1 : this.Control2;
				c.Tags.AddStyleTag(".r", new SciTags.UserDefinedStyle { textColor = 0xf08080 }); //red regex
				c.Tags.AddLinkTag("+p", o => CurrentTopic = o); //link to a local info topic
				c.Tags.SetLinkStyle(new SciTags.UserDefinedStyle { textColor = 0x0080FF, underline = false }); //remove underline from links
				c.Call(Sci.SCI_SETWRAPSTARTINDENT, 4);
			}
			this.Control2.Tags.AddStyleTag(".h", new SciTags.UserDefinedStyle { backColor = 0xC0E0C0, bold = true, eolFilled = true }); //topic header
			this.Control2.Tags.AddLinkTag("+a", o => _Insert(o)); //link that inserts a regex token

			_SetTocText();
			CurrentTopic = "help";
		}

		string _GetContentText()
		{
			var s = ContentText ?? Properties.Resources.Regex;
			if(!s.Has('\n')) s = File.ReadAllText(s);
			return s;
		}

		void _SetTocText()
		{
			var s = _GetContentText();
			s = s.Remove(s.Find("\r\n\r\n-- "));
			this.Text = s;
		}

		/// <summary>
		/// Opens an info topic or gets current topic name.
		/// </summary>
		public string CurrentTopic {
			get => _topic;
			set {
				if(value == _topic) return;
				_topic = value;
				var s = _GetContentText();
				if(!s.RegexMatch($@"(?ms)^-- {_topic} --\R\R(.+?)\R-- ", 1, out s)) s = "";
				this.Text2 = s;
			}
		}
		string _topic;

		public void Refresh()
		{
			_SetTocText();
			var s = _topic;
			_topic = null;
			CurrentTopic = s;
		}

		/// <summary>
		/// Content text or file path.
		/// If changed later, then call Refresh.
		/// If null (default), uses text from resources of this dll.
		/// </summary>
		public string ContentText { get; set; }

		/// <summary>
		/// A text control in which to insert the regex token when the link clicked.
		/// If null, uses the focused control.
		/// </summary>
		public Control InsertInControl { get; set; }

		void _Insert(string rx)
		{
			var c = InsertInControl;
			if(c == null) {
				c = AWnd.ThisThread.FocusedControl;
				if(c == null) return;
			} else c.Focus();

			Task.Run(() => {
				int i = rx.IndexOf('%');
				if(i >= 0) {
					Debug.Assert(!rx.Has('\r'));
					rx = rx.Remove(i, 1);
					i = rx.Length - i;
				}
				var k = new AKeys(null);
				k.AddText(rx);
				if(i > 0) k.AddKey(KKey.Left).AddRepeat(i);
				k.Send();
			});
		}
	}
}
