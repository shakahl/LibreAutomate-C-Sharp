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
	public class WindowTriggers :ITriggers
	{
		class _TriggerEtc : TriggerBase
		{
			public readonly Wnd.Finder finder;

			public _TriggerEtc(Wnd.Finder finder, Action<AutotextTriggerArgs> action) : base(action)
			{
				this.finder = finder;
			}

			public override void Run(int data1, string data2, Wnd w)
			{
				Print(data1, data2, w);
			}
		}

		ETriggerType _ttype;
		List<_TriggerEtc> _a = new List<_TriggerEtc>();

		internal WindowTriggers(ETriggerType ttype)
		{
			_ttype = ttype;
		}

		public Action<TriggerArgs> this[string name, string className = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null] {
			set {

			}
		}

		ETriggerEngineProcess ITriggers.EngineProcess => _a.Count > 0 ? ETriggerEngineProcess.Local : ETriggerEngineProcess.None;

		void ITriggers.Write(BinaryWriter w)
		{
			//TODO: set hook
		}

		bool ITriggers.CanRun(int action, int data1, string data2, Wnd w, WFCache cache) => true;

		TriggerBase ITriggers.GetAction(int action) => _a[action];
	}
}
