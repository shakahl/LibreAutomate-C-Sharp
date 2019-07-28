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
using static Au.AStatic;
using System.Globalization;

static class LibLog
{
	public static LogFile Run { get; } = new LogFile(AFolders.ThisAppTemp + "run.log");

	public class LogFile //:IDisposable
	{
		string _path;
		StreamWriter _writer;
		Mutex _mutex;

		public LogFile(string path)
		{
			_path = path;
		}

		//public void Dispose()
		//{
		//	_writer?.Dispose(); _writer = null;
		//	_mutex?.Dispose(); _mutex = null;
		//}

		public string FilePath => _path;

		public void Write(string s)
		{
			Debug.Assert(ATask.Role == ATRole.MiniProgram); //could be any, but currently we log only in miniProgram processes. Would be no task name if called in editor process.
			try {
				if(_writer == null || _mutex == null) {
					lock(this) {
						if(_writer == null) {
							_writer = new StreamWriter(new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite));
						}
						if(_mutex == null) {
							_mutex = new Mutex(false, "Au.Mutex.LogFile");
						}
					}
				}
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
