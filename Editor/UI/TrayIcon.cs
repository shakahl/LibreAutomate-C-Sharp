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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using static Program;
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

#if true
	static AMenu _menu;

	public static void Add()
	{
		_trayIcon = new NotifyIcon();
		_trayIcon.MouseClick += _trayIcon_MouseClick;
		_trayIcon.Icon = EdStock.IconTrayNormal;
		_trayIcon.Text = Program.AppName;

		_menu = new AMenu { MultiShow = true, ActivateMenuWindow = true };
		_menu["End green task\tSleep"] = o => Tasks.EndTask();
		_menu["Disable triggers\tM-click"] = o => Run.DisableTriggers(null); var dt = _menu.LastMenuItem;
		_menu.CMS.Opening += (unu, sed) => { dt.Checked = _disabled; };
		_menu.Separator();
		_menu["Exit"] = o => Strips.Cmd.File_Exit();
		_trayIcon.ContextMenuStrip = _menu.CMS;

		_trayIcon.Visible = true;
	}

	static void _trayIcon_MouseClick(object sender, MouseEventArgs e)
	{
		//Print(e.Button);
		switch(e.Button) {
		case MouseButtons.Left:
			MainForm.Visible = true;
			var w = (AWnd)MainForm;
			w.ShowNotMinimized(true);
			w.ActivateLL();
			break;
		case MouseButtons.Middle:
			Run.DisableTriggers(null);
			break;
		}
	}
#else //the above version is better because activates previous window when context menu closed. Also, it's standard, therefore possibly more reliable.
	public static void Add()
	{
		_trayIcon = new NotifyIcon();
		_trayIcon.MouseClick += _trayIcon_MouseClick;
		_trayIcon.Icon = EdStock.IconTrayNormal;
		_trayIcon.Text = Program.AppName;
		_trayIcon.Visible = true;
	}

	static void _trayIcon_MouseClick(object sender, MouseEventArgs e)
	{
		//Print(e.Button);
		switch(e.Button) {
		case MouseButtons.Left:
			MainForm.Visible = true;
			var w = (AWnd)MainForm;
			w.ShowNotMinimized(true);
			w.ActivateLL();
			break;
		case MouseButtons.Right:
			var m = new AMenu { ActivateMenuWindow = true };
			m["End green task\tSleep"] = o => Tasks.EndTask();
			m["Disable triggers\tM-click"] = o => Run.DisableTriggers(null);
			m.LastMenuItem.Checked = _disabled;
			m.Separator();
			m["Exit"] = o => Strips.Cmd.File_Exit();
			m.Show();
			//never mind: without ActivateMenuWindow does not show second time. Shows first, third, etc, but not second.
			//	.NET somehow closes CMS, and then AMenu disposes self.
			//	Difficult to debug, because VS now does not show .NET source code when debugging.
			//	Only when used with NotifyIcon.
			break;
		case MouseButtons.Middle:
			Run.DisableTriggers(null);
			break;
		}
	}
#endif

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
