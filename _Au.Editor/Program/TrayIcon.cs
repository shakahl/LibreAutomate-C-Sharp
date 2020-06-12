using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

static class EdTrayIcon
{
	static NotifyIcon _trayIcon;
	static bool _disabled, _running;

	public static void Dispose()
	{
		_trayIcon?.Dispose();
		_trayIcon = null;
	}

	public static void Add()
	{
		_trayIcon = new NotifyIcon();
		_trayIcon.MouseClick += _trayIcon_MouseClick;
		_trayIcon.Icon = EdStock.IconTrayNormal;
		_trayIcon.Text = Program.AppName;
		_trayIcon.Visible = true;

		var m = new AMenu { MultiShow = true, ActivateMenuWindow = true };
		m["End green task\tSleep"] = o => Program.Tasks.EndTask();
		m["Disable triggers\tM-click"] = o => TriggersAndToolbars.DisableTriggers(null); var dt = m.LastMenuItem;
		m.Control.Opening += (_, _) => { dt.Checked = _disabled; };
		m.Separator();
		m["Exit"] = o => Strips.Cmd.File_Exit();
		_trayIcon.ContextMenuStrip = m.Control;
	}

	static void _trayIcon_MouseClick(object sender, MouseEventArgs e)
	{
		//AOutput.Write(e.Button);
		switch(e.Button) {
		case MouseButtons.Left:
			Program.MainForm.ZShowAndActivate();
			break;
		case MouseButtons.Middle:
			TriggersAndToolbars.DisableTriggers(null);
			break;
		}
	}

	public static bool Disabled {
		get => _disabled;
		set {
			if(value != _disabled) {
				_disabled = value;
				if(!_running) _UpdateIcon();
			}
		}
	}

	public static bool Running {
		get => _running;
		set {
			if(value != _running) {
				_running = value;
				_UpdateIcon();
			}
		}
	}

	static void _UpdateIcon()
	{
		_trayIcon.Icon = _running ? EdStock.IconAppRunning : (_disabled ? EdStock.IconAppDisabled : EdStock.IconTrayNormal);
	}
}
