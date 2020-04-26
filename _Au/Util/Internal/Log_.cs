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
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	static class Log_
	{
		public static LogFile Run { get; } = new LogFile("run.log");

		public class LogFile //:IDisposable
		{
			readonly string _name;
			string _path;
			Handle_ _hfile; //simpler and faster than with StreamWriter
			IntPtr _mutex; //simpler and faster than with Mutex. Mutex.WaitOne very slow first time in STA thread.
			volatile int _openState; //1 queing work item, 2 opening, 3 is open, -1 failed to open
			StringBuilder _sb;

			public LogFile(string name)
			{
				_name = name;
			}

			//public void Dispose()
			//{
			//	_hfile.Dispose();
			//	Api.CloseHandle(_mutex); _mutex = default;
			//}

			//public string FilePath => _Path;

			string _Path => _path ??= AFolders.ThisAppTemp + _name; //info: ThisAppTemp slow first time

			public void Start()
			{
				try { File.WriteAllText(_Path, ""); } catch { }
			}

			public void Show()
			{
				if(_path != null && AFile.ExistsAsFile(_path)) AFile.TryRun(_path);
			}

			public unsafe void Write(string s)
			{
				//using var p1 = APerf.Create();
				Debug.Assert(ATask.Role == ATRole.MiniProgram); //could be any, but currently we log only in miniProgram processes. Would be no task name if called in editor process.

				if(_openState != 3) {
					//First time open and write async. Else it would be the slowest part of starting a preloaded assembly in Au.Task.exe.
					if(0 == Interlocked.CompareExchange(ref _openState, 1, 0)) {
						new Thread(() => {
							//ThreadPool.QueueUserWorkItem(_ => { //several times slower, eg 1500 vs 400
							lock(this) {
								_openState = 2;
								_hfile = Api.CreateFile(_Path, Api.GENERIC_WRITE, Api.FILE_SHARE_READ | Api.FILE_SHARE_WRITE, default, Api.OPEN_ALWAYS);
								if(_hfile.Is0) {
									ADebug.Print("failed");
									_openState = -1;
									return;
								}
								_mutex = Api.CreateMutex(null, false, "Au.Mutex.LogFile");
								_Write(s);
								_openState = 3;
							}
							//});
						}).Start();
						return;
					}
					while(_openState == 1) Thread.Sleep(15);
					lock(this) {
						if(_openState < 0) return;
					}
				}
				_Write(s);
			}

			unsafe void _Write(string s)
			{
				int r = Api.WaitForSingleObject(_mutex, -1);
				ADebug.PrintIf(r != 0, r);
				try {
					var b = _sb ??= new StringBuilder();
					b.Clear();
#if true
					var sa = stackalloc char[32];
					Api.GetLocalTime(out var t);
					int nf = Api.wsprintfW(sa, "%i-%02i-%02i %02i:%02i:%02i.%03i | ", __arglist(t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond, t.wMilliseconds));
					b.Append(sa, nf);
#else //this would be the slowest part until tiered JIT optimization, and 3 times slower later
					var dt = DateTime.Now;
					var sd = dt.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
					b.Append(sd);
#endif
					b.Append(ATask.Name).Append(" | ").AppendLine(s);
					bool preloading = s == null;
					s = b.ToString();
					var a = Encoding.UTF8.GetBytes(s); //slow until tiered JIT optimization
					if(preloading) a = Array.Empty<byte>();
					Api.SetFilePointerEx(_hfile, 0, null, Api.FILE_END); //always, because our file pointer isn't moved when others write
					Api.WriteFileArr(_hfile, a, out _);
				}
				finally { Api.ReleaseMutex(_mutex); }
			}
		}
	}
}
