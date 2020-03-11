using System;
using System.Collections.Generic;
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
//using System.Linq;

using Au.Types;
using System.Globalization;

namespace Au.Util
{
	static class Log_
	{
		public static LogFile Run { get; } = new LogFile("run.log");

		public class LogFile //:IDisposable
		{
			string _name, _path;
			StreamWriter _writer;
			Mutex _mutex;
			volatile int _openState; //1 queing work item, 2 opening, 3 is open, -1 failed to open

			public LogFile(string name)
			{
				_name = name;
			}

			//public void Dispose()
			//{
			//	_writer?.Dispose(); _writer = null;
			//	_mutex?.Dispose(); _mutex = null;
			//}

			//public string FilePath => _Path;

			string _Path => _path ??= AFolders.ThisAppTemp + _name; //info: ThisAppTemp slow first time

			public void Start()
			{
				try { File.WriteAllText(_Path, ""); } catch { }
			}

			public void Show()
			{
				if(_path != null && AFile.ExistsAsFile(_path)) AExec.TryRun(_path);
			}

			public void Write(string s)
			{
				Debug.Assert(ATask.Role == ATRole.MiniProgram); //could be any, but currently we log only in miniProgram processes. Would be no task name if called in editor process.

				if(_openState != 3) {
					//First time open and write async. Else it would be the slowest part of starting a preloaded assembly in Au.Task.exe.
					if(0 == Interlocked.CompareExchange(ref _openState, 1, 0)) {
						ThreadPool.QueueUserWorkItem(_ => {
							lock(this) {
								_openState = 2;
								try {
									_mutex = new Mutex(false, "Au.Mutex.LogFile");
									_writer = new StreamWriter(new FileStream(_Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite));
								}
								catch(Exception ex) {
									ADebug.Print(ex.ToStringWithoutStack());
									_openState = -1;
									return;
								}
								_Write(s);
								_openState = 3;
							}
						});
						return;
					}
					while(_openState == 1) Thread.Sleep(15);
					lock(this) {
						if(_openState < 0) return;
					}
				}
				_Write(s);
			}

			void _Write(string s)
			{
				try {
					try { _mutex.WaitOne(); } catch(AbandonedMutexException) { }
					try {
						_writer.BaseStream.Seek(0, SeekOrigin.End);
						_writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo));
						_writer.Write(" | ");
						_writer.Write(ATask.Name);
						_writer.Write(" | ");
						_writer.WriteLine(s);
						_writer.Flush();
					}
					finally { _mutex.ReleaseMutex(); }
				}
				catch(Exception ex) { ADebug.Print(ex.ToStringWithoutStack()); }
			}
		}
	}
}
