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
	public class AutotextTriggers : ITriggers
	{
		class _TriggerEtc : Trigger
		{
			string _shortString;

			internal _TriggerEtc(Triggers triggers, Action<AutotextTriggerArgs> action, string text) : base(triggers, action, true)
			{
				_shortString = text;
			}

			internal override void Run(TriggerArgs args) => RunT(args as AutotextTriggerArgs);

			internal override string TypeString() => "Autotext";

			internal override string ShortString() => _shortString;
		}

		Triggers _triggers;
		Dictionary<string, Trigger> _d = new Dictionary<string, Trigger>();

		internal AutotextTriggers(Triggers triggers)
		{
			_triggers = triggers;
		}

		public Action<AutotextTriggerArgs> this[string text] {
			set {
				var t = new _TriggerEtc(_triggers, value, text);
				t.DictAdd(_d, text);
			}
		}

		bool ITriggers.HasTriggers => _d.Count > 0;

		void ITriggers.StartStop(bool start)
		{

		}

		internal bool HookProc(HookData.Keyboard k, TriggerHookContext thc)
		{
			//note: this is called after HotkeyTriggers.HookProc.
			//	It may set thc.triggers and return false to not suppress the input event. Then we should reset autotext.

			if(!k.IsInjectedByAu && !k.IsUp && 0 == k.Mod) {

			}
			return false;
		}
	}

	public class AutotextTriggerArgs : TriggerArgs
	{
		internal AutotextTriggerArgs(Trigger trigger) : base(trigger)
		{
		}

		public void Replace(string replacement)
		{

		}
	}
}
