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
using static Au.AStatic;
using Au.Triggers;

//TODO: if registry "Control Panel\Desktop:LowLevelHooksTimeout" < 5000, print a note with a link to correct it.

//class TriggersUI
//{
//	readonly TriggerEngines _triggers;

//	/// <summary>
//	/// Called when workspace loaded.
//	/// </summary>
//	public TriggersUI(FilesModel model)
//	{
//		var dbPath = model.WorkspaceDirectory + @"\triggers.db";
//		_triggers = new TriggerEngines(dbPath);




//	}

//	/// <summary>
//	/// Called when workspace closing.
//	/// </summary>
//	public void Dispose()
//	{
//		_triggers.Dispose();

//	}
//}
