using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Xml.XPath;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;
using static Au.Controls.Sci;

class PanelOutput :Control
{
	SciOutput _c;
	Queue<Output.Server.Message> _history;

	//public SciControl Output { get => _c; }

	public PanelOutput()
	{
		_c = new SciOutput();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Output";
		this.Controls.Add(_c);

		_history = new Queue<Output.Server.Message>();
		OutputServer.SetNotifications(_GetServerMessages, this);

		_c.HandleCreated += _c_HandleCreated;
	}

	void _GetServerMessages()
	{
		_c.Tags.OutputServerProcessMessages(OutputServer, m =>
		{
			if(m.Type != Output.Server.MessageType.Write) return;
			_history.Enqueue(m);
			if(_history.Count > 50) _history.Dequeue();
		});
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void Clear() { _c.ST.ClearText(); }

	public void Copy() { _c.Call(SCI_COPY); }

	//not override void OnHandleCreated, because then _c handle still not created, and we need to Call
	private void _c_HandleCreated(object sender, EventArgs e)
	{
		var h = _c.Handle;
		_inInitSettings = true;
		if(WrapLines) WrapLines = true;
		if(WhiteSpace) WhiteSpace = true;
		if(Topmost) Strips.CheckCmd("Tools_Output_Topmost", true); //see also OnParentChanged, below
		_inInitSettings = false;
	}
	bool _inInitSettings;

	public bool WrapLines
	{
		get => Settings.Get("Tools_Output_WrapLines", false);
		set
		{
			Debug.Assert(!_inInitSettings || value);
			if(!_inInitSettings) Settings.Set("Tools_Output_WrapLines", value);
			//_c.Call(SCI_SETWRAPVISUALFLAGS, SC_WRAPVISUALFLAG_START | SC_WRAPVISUALFLAG_END); //in SciControl.OnHandleCreated
			//_c.Call(SCI_SETWRAPINDENTMODE, SC_WRAPINDENT_INDENT); //in SciControl.OnHandleCreated
			_c.Call(SCI_SETWRAPMODE, value ? SC_WRAP_WORD : 0);
			Strips.CheckCmd("Tools_Output_WrapLines", value);
		}
	}

	public bool WhiteSpace
	{
		get => Settings.Get("Tools_Output_WhiteSpace", false);
		set
		{
			Debug.Assert(!_inInitSettings || value);
			if(!_inInitSettings) Settings.Set("Tools_Output_WhiteSpace", value);
			_c.Call(SCI_SETWHITESPACEFORE, 1, 0xFF0080);
			_c.Call(SCI_SETVIEWWS, value);
			Strips.CheckCmd("Tools_Output_WhiteSpace", value);
		}
	}

	public bool Topmost
	{
		get => Settings.Get("Tools_Output_Topmost", false);
		set
		{
			var p = Panels.PanelManager.GetPanel(this);
			if(value) p.Floating = true;
			if(p.Floating) _SetTopmost(value);
			Settings.Set("Tools_Output_Topmost", value);
			Strips.CheckCmd("Tools_Output_Topmost", value);
		}
	}

	void _SetTopmost(bool on)
	{
		var w = ((Wnd)this).WndWindow;
		if(on) {
			w.WndOwner = default;
			w.ZorderTopmost();
			//w.SetExStyle(Native.WS_EX_APPWINDOW, SetAddRemove.Add);
			//Wnd.Misc.WndRoot.ActivateLL(); w.ActivateLL(); //let taskbar add button
		} else {
			w.ZorderNoTopmost();
			w.WndOwner = MainForm.Wnd_();
		}
	}

	protected override void OnParentChanged(EventArgs e)
	{
		if(Parent is Form && Topmost) Timer_.After(1, t => _SetTopmost(true));

		base.OnParentChanged(e);
	}

	class SciOutput :AuScintilla
	{
		public SciOutput()
		{
			InitReadOnlyAlways = true;
			InitTagsStyle = TagsStyle.AutoWithPrefix;
			InitImagesStyle = ImagesStyle.ImageTag;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			ST.MarginWidth(1, 3);
			ST.StyleBackColor(STYLE_DEFAULT, 0xF7F7F7);
			ST.StyleFont(STYLE_DEFAULT, "Courier New", 8);
			ST.StyleClearAll();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			switch(e.Button) {
			case MouseButtons.Middle:
				ST.ClearText();
				break;
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			switch(e.Button) {
			case MouseButtons.Right:
				Strips.ddOutput.ShowAsContextMenu_();
				break;
			}
			base.OnMouseUp(e);
		}
	}
}
