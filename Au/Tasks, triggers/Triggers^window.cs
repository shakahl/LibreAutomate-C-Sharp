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

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	public abstract class WindowTriggers
	{
		struct _TriggerEtc
		{
			public Wnd.Finder finder;
			public Action<TriggerArgs> action;
		}

		List<_TriggerEtc> _a = new List<_TriggerEtc>();

		public Action<TriggerArgs> this[string name, string className = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null] {
			set {

			}
		}

		private protected EEngineProcess EngineProcess => _a.Count > 0 ? EEngineProcess.Local : EEngineProcess.None;

		private protected void Write(BinaryWriter w)
		{
			//TODO: set hook
		}

	}

	public class WindowCreatedTriggers : WindowTriggers, ITriggers
	{
		internal WindowCreatedTriggers()
		{

		}

		EEngineProcess ITriggers.EngineProcess => base.EngineProcess;

		void ITriggers.Write(BinaryWriter w) => base.Write(w);

		bool ITriggers.CanRun(int action, int data1, string data2, Wnd w, WFCache cache) => true;

		void ITriggers.Run(int action, int data1, string data2, Wnd w)
		{
			Print(action, data1, data2, w);
		}

	}
}
