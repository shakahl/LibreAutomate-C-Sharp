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
		internal struct Trigger
		{
			public string text;
		}

		struct _TriggerEtc
		{
			public Trigger trigger;
			public int scope; //0 or Triggers.Of.Index
			public Action<AutotextTriggerArgs> action;
		}

		List<_TriggerEtc> _a = new List<_TriggerEtc>();

		internal AutotextTriggers()
		{

		}

		public Action<AutotextTriggerArgs> this[string text] {
			set {
				var t = new Trigger { text = text };
				_a.Add(new _TriggerEtc { trigger = t, scope = Triggers.Of.Index, action = value });
			}
		}

		EEngineProcess ITriggers.EngineProcess => _a.Count > 0 ? EEngineProcess.Remote : EEngineProcess.None;

		void ITriggers.Write(BinaryWriter w)
		{
			w.Write(_a.Count);
			foreach(var v in _a) w.Write(v.trigger.text);
		}

		bool ITriggers.CanRun(int action, int data1, string data2, Wnd w, WFCache cache)
		{
			var v = _a[action];
			if(!Triggers.Of.Match(v.scope, w, cache)) return false;
			return true;
		}

		void ITriggers.Run(int action, int data1, string data2, Wnd w)
		{
			Print(action, data1, data2, w);
		}

	}

	class AutotextTriggersEngine : ITriggersEngine
	{
		class _TriggerValue
		{
			public _TriggerValue(int pipe, int action)
			{
				PipeIndex = pipe; ActionIndex = action; //this.flags = flags;
			}

			public int PipeIndex { get; }
			public int ActionIndex { get; }
			//public XTriggers.TFlags flags;
		}

		Dictionary<string, object> _d;

		public void Dispose()
		{
			KeyboardHook.Instance?.Dispose();
			_d?.Clear();
		}

		void ITriggersEngine.AddTriggers(int pipeIndex, BinaryReader r, byte[] raw)
		{
			if(_d == null) _d = new Dictionary<string, object>();
			int n = r.ReadInt32();
			for(int i = 0; i < n; i++) {
				AutotextTriggers.Trigger t = default;
				t.text = r.ReadString();
				var k = t.text;
				var v = new _TriggerValue(pipeIndex, i);
				if(!_d.TryGetValue(k, out var o)) _d.Add(k, v);
				else if(o is List<_TriggerValue> m) m.Add(v);
				else _d[k] = new List<_TriggerValue> { o as _TriggerValue, v };
			}
		}

		void ITriggersEngine.RemoveTriggers(int pipe)
		{
			//TODO
		}

		internal bool HookProc(HookData.Keyboard k)
		{
			if(!k.IsUp && k.Mod == 0) {
				//Print(k);

				//var b = new byte[100];
				//TriggersInEditor.Instance.SendEvent(0, b);
			}
			return false;
		}
	}

	public class AutotextTriggerArgs : TriggerArgs
	{

		public void Replace(string replacement)
		{

		}
	}
}
