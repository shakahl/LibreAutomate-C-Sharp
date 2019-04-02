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
	/// <summary>
	/// Represents an autotext trigger.
	/// </summary>
	public class AutotextTrigger : ActionTrigger
	{
		string _paramsString;

		internal AutotextTrigger(ActionTriggers triggers, Action<AutotextTriggerArgs> action, string text) : base(triggers, action, true)
		{
			_paramsString = text;
		}

		internal override void Run(TriggerArgs args) => RunT(args as AutotextTriggerArgs);

		/// <inheritdoc/>
		public override string TypeString => "Autotext";

		/// <inheritdoc/>
		public override string ParamsString => _paramsString;
	}

	/// <summary>
	/// Autotext triggers.
	/// </summary>
	/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
	public class AutotextTriggers : ITriggers
	{
		ActionTriggers _triggers;
		Dictionary<string, ActionTrigger> _d = new Dictionary<string, ActionTrigger>();

		internal AutotextTriggers(ActionTriggers triggers)
		{
			_triggers = triggers;
		}

		public Action<AutotextTriggerArgs> this[string text] {
			set {
				_triggers.LibThrowIfRunning();
				var t = new AutotextTrigger(_triggers, value, text);
				t.DictAdd(_d, text);
				_lastAdded = t;
			}
		}

		/// <summary>
		/// The last added trigger.
		/// </summary>
		public AutotextTrigger Last => _lastAdded;
		AutotextTrigger _lastAdded;

		bool ITriggers.HasTriggers => _lastAdded != null;

		void ITriggers.StartStop(bool start)
		{

		}

		internal bool HookProc(HookData.Keyboard k, TriggerHookContext thc)
		{
			//note: this is called after HotkeyTriggers.HookProc.
			//	It may set thc.triggers and return false to not suppress the input event. Then we should reset autotext.

			Debug.Assert(!k.IsInjectedByAu); //server must ignore

			if(!k.IsUp && 0 == k.Mod) {

			}
			return false;
		}
	}

	/// <summary>
	/// Arguments for actions of autotext triggers.
	/// </summary>
	public class AutotextTriggerArgs : TriggerArgs
	{
		public AutotextTrigger Trigger { get; internal set; }
		public Wnd Window { get; }

		///
		public AutotextTriggerArgs(AutotextTrigger trigger, Wnd w)
		{
			Trigger = trigger;
			Window = w;
		}

		public void Replace(string replacement)
		{

		}
	}
}
