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

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Triggers
{
    public interface ITriggerEngine
    {
		#region engine

		//void Load(SqliteDB db);
		//void Save(SqliteDB db);
		void Start(SqliteDB db);
		void Stop();
		bool Paused { get; set; }

		#endregion

		#region trigger

		void Add(object trigger, uint fileId, string method);
		void Remove(object trigger, uint fileId, string method);
		void DisableEnable(bool disable, uint fileId, string method);

		#endregion

		#region UI

		void FormInit(System.Windows.Forms.Form parent);
		void FormOK();
		void Help();
		System.Drawing.Icon Icon { get; set; }

		#endregion
	}
}
