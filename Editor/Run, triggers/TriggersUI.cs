//#define ADMIN_TRIGGERS

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Triggers;
using static Program;

//TODO: if registry "Control Panel\Desktop:LowLevelHooksTimeout" < 5000, print a note with a link to correct it.

class TriggersUI
{
	readonly Triggers _triggers;
#if ADMIN_TRIGGERS
	//Process _adminProcess;
	bool _useAdminProcess;
#endif

	/// <summary>
	/// Called when workspace loaded.
	/// </summary>
	public TriggersUI(FilesModel model)
	{
		var dbPath = model.WorkspaceDirectory + @"\triggers.db";
#if ADMIN_TRIGGERS
		_useAdminProcess = Settings.Get("triggersAdmin", false) && IpcWithHI.CanRunHi();
		if(_useAdminProcess) {
			IpcWithHI.Call(101, (int)MainForm.Handle, dbPath);
		} else {
#endif
		_triggers = new Triggers(dbPath);

#if ADMIN_TRIGGERS
		}
#endif




	}

	/// <summary>
	/// Called when workspace closing.
	/// </summary>
	public void Dispose()
	{
#if ADMIN_TRIGGERS
		if(_useAdminProcess) {
			IpcWithHI.CallIfRunning(102, (int)MainForm.Handle);
		} else {
#endif
		_triggers.Dispose();

#if ADMIN_TRIGGERS
		}
#endif
	}
}
