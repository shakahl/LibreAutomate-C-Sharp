using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using System.Reflection;
using System.Diagnostics;

//using System.ComponentModel; //EditorBrowsableAttribute

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Automation
{
	public static class Input
    {
		//VS 2015 bug: EditorBrowsableAttribute does not work. I tried to hide Equals and ReferenceEquals in intellisense lists. Note: in Options is checked 'Hide advanced members'.

		//[EditorBrowsable(EditorBrowsableState.Never)]
		////[EditorBrowsable(EditorBrowsableState.Advanced)]
		//static new bool Equals(object a, object b) { return false; }

		//[EditorBrowsable(EditorBrowsableState.Never)]
		////[EditorBrowsable(EditorBrowsableState.Advanced)]
		//static new bool ReferenceEquals(object a, object b) { return false; }

		////[EditorBrowsable(EditorBrowsableState.Never)]
		//////[EditorBrowsable(EditorBrowsableState.Advanced)]
		////public override bool Equals(object a) { return false; }



		//public static void Keys(params object[] keys)
		//{
		//	//info:
		//	//Named not Keys because then hides enum Keys from System.Windows.Forms. Also various interfaces have members named Keys.
		//	//Named not SendKeys because too long and hides class SendKeys from System.Windows.Forms.
		//	//Never mind: Key hides enum Key from Microsoft.DirectX.DirectInput and System.Windows.Input. Also various classes have Key property.

		//	//Out(keys.Length);
		//	foreach(object o in keys) {
		//		//switch(o.GetType().
		//		if(o is string) {
		//			var s=o as string;
		//			Out($"string: {s}");
		//		} else if(o is KeysToSend) {
		//			var k=o as KeysToSend;
		//			Out($"KeysToSend: len={k.Length}");
		//		} else if(o is double) {
		//			var d=(double)o;
		//			Out($"double: {d}");
		//		} else if(o is int) {
		//			var i=(int)o;
		//			Out($"int: {i}");
		//		} else {
		//			Out("error");
		//		}
		//	}
		//}

		public static void Keys(params string[] keys_text_keys_text_andSoOn)
		{
		}

		//public static void Key(params string[] keys_text_keys_text_andSoOn)
		//{
		//}

		//Uses Script.Option.hybridText.
		public static void Text(params string[] text_keys_text_keys_andSoOn)
		{
		}

		public static void Text(bool hybrid, params string[] text_keys_text_keys_andSoOn)
		{
		}

		public static void Text(ScriptOptions options, params string[] text_keys_text_keys_andSoOn)
		{
		}


		//public class KeysToSend
		//{
		//	List<int> _a;

		//	public int Length { get { return _a.Count; } }

		//	public KeysToSend Tab { get { _a.Add(9); return this; } }
		//	public KeysToSend Enter { get { _a.Add(13); return this; } }
		//	public KeysToSend Ctrl { get { _a.Add(1); return this; } }
		//	public KeysToSend A { get { _a.Add('A'); return this; } }
		//	//public KeysToSend Tab(int nTimes) { return 0; } //error
		//	public KeysToSend Repeat(int nTimes) { _a.Add(-nTimes); return this; }

		//}

		//public static KeysToSend K { get { return new KeysToSend(); } }


		static void Test()
		{
			//Key("text", K.Ctrl.A.Tab.Repeat(9).Enter);
		}
    }
}
