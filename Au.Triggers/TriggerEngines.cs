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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Triggers
{
	public class TriggerEngines
	{
		Thread _thread;
		AutoResetEvent _endEvent;
		SqliteDB _db;
		//HotkeyTriggersEngine _trigHotkey;

		public TriggerEngines(string dbPath)
		{
			//open database of triggers
			try {
				_db = new SqliteDB(dbPath, SLFlags.SQLITE_OPEN_READONLY | SLFlags.SQLITE_OPEN_NOMUTEX);
			}
			catch(SLException ex) { if(File_.ExistsAsAny(dbPath)) Print(ex); return; }

			//start thread
			_endEvent = new AutoResetEvent(false);
			_thread = Thread_.Start(_Thread);
		}

		public void Dispose()
		{
			if(_db == null) return;

			//end thread
			_endEvent.Set();
			_thread.Join();

			//close database
			_db.Dispose();
			_db = null;
		}

		void _Thread()
		{
			var hotkey = new HotkeyTriggersEngine();
			lock(_db) {
				hotkey.Start(_db);
			}
			try {
				_endEvent.WaitOne();
			}
			finally {
				hotkey.Stop();
			}
		}
	}

	public class TriggerEventArgs :EventArgs
	{
		public uint fileId;
		public string method;
	}

	public class HotkeyTriggersEngine :ITriggerEngine
	{
		Au.Util.WinHook _hook;
		Dictionary<int, object> _d;

		#region engine

		public void Start(SqliteDB db)
		{
			Debug.Assert(_hook == null && _d == null);

			_d = new Dictionary<int, object>();
			try {
				using(var x = db.Statement("SELECT * FROM _hotkey")) {
					while(x.Step()) {
						Print(x.ColumnCount);
					}
				}
			}
			catch(SLException ex) when(ex.ErrorCode == SLError.Error && !db.TableExists("_hotkey")) {
				//db.Execute("CREATE TABLE _hotkey(key INT, file INT, method TEXT, flags INT, wndName TEXT, wndClass TEXT, program TEXT)");
			}
			if(_d.Count == 0) return;

			_hook = Au.Util.WinHook.Keyboard(_HookProc);
		}

		public void Stop()
		{
			if(_hook != null) {
				_hook.Dispose();
				_hook = null;
			}
			_d = null;
		}

		bool _HookProc(HookData.Keyboard d)
		{
			//Print(d);
			return false;
		}

		public bool Paused { get; set; }

		#endregion

		#region trigger

		public void Add(object trigger, uint fileId, string method)
		{
			
		}

		public void Remove(object trigger, uint fileId, string method)
		{
			
		}

		public void DisableEnable(bool disable, uint fileId, string method)
		{
			
		}

		#endregion

		//#region UI

		//public void FormInit(Form parent)
		//{
			
		//}

		//public void FormOK()
		//{
			
		//}

		//public void Help()
		//{
			
		//}

		//public Icon Icon { get; set; }

		//#endregion
	}
}
