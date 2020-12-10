using System;
using System.IO;
using System.Windows.Forms;

using Au.Types;
using Au.Controls;
using Au.Util;

namespace Au.Tools
{
	class RegexWindow : InfoWindow
	{
		public RegexWindow(AWnd ownerForDpi)
		{
			int dpi = ADpi.OfWindow(ownerForDpi);
			base.InitTwoControlsSplitPos = ADpi.Scale(250, dpi);
			this.Size = ADpi.Scale((800, 220), dpi);
			this.Caption = "Regex";
		}

		protected override void OnLoad(EventArgs e)
		{
			for(int i = 0; i < 2; i++) {
				var c = i == 0 ? this.Control1 : this.Control2;
				c.ZTags.AddStyleTag(".r", new SciTagsF.UserDefinedStyle { textColor = 0xf08080 }); //red regex
				c.ZTags.AddLinkTag("+p", o => CurrentTopic = o); //link to a local info topic
				c.ZTags.SetLinkStyle(new SciTagsF.UserDefinedStyle { textColor = 0x0080FF, underline = false }); //remove underline from links
				c.Call(Sci.SCI_SETWRAPSTARTINDENT, 4);
			}
			this.Control2.ZTags.AddStyleTag(".h", new SciTagsF.UserDefinedStyle { backColor = 0xC0E0C0, bold = true, eolFilled = true }); //topic header
			this.Control2.ZTags.AddLinkTag("+a", o => InsertCode.TextSimplyInControl(InsertInControl, o)); //link that inserts a regex token

			_SetTocText();
			CurrentTopic = "help";
		}

		string _GetContentText()
		{
			var s = ContentText ?? AResources.GetString("tools/regex.txt");
			if(!s.Contains('\n')) s = File.ReadAllText(s);
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
	}
}
