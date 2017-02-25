using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

#pragma warning disable 1591 //XML doc. //TODO

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



		public static void Keys(params string[] keys_text_keys_text_andSoOn)
		{
			var keys = keys_text_keys_text_andSoOn;
			if(keys == null) return;
		}

		//note:
		//	Don't use the hybrid option in Catkeys. In many apps sending keys for text snippets etc is too slow, better to paste always.
		//	Then probably don't need Text(). In that rare cases when need, can use Keys("", "text");
		//public static void Text(params string[] text_keys_text_keys_andSoOn)
		//{
		//	var keys = text_keys_text_keys_andSoOn;
		//	if(keys == null) return;
		//}

		//public static void Text(bool hybrid, params string[] text_keys_text_keys_andSoOn)
		//{
		//	var keys = text_keys_text_keys_andSoOn;
		//	if(keys == null) return;
		//}

		//public static void Text(ScriptOptions options, params string[] text_keys_text_keys_andSoOn)
		//{
		//	var keys = text_keys_text_keys_andSoOn;
		//	if(keys == null) return;
		//}


		//public class KeysToSend
		//{
		//	List<int> _a;

		//	public int Length { get => _a.Count; }

		//	public KeysToSend Tab { get { _a.Add(9); return this; } }
		//	public KeysToSend Enter { get { _a.Add(13); return this; } }
		//	public KeysToSend Ctrl { get { _a.Add(1); return this; } }
		//	public KeysToSend A { get { _a.Add('A'); return this; } }
		//	//public KeysToSend Tab(int nTimes) { return 0; } //error
		//	public KeysToSend Execute(int nTimes) { _a.Add(-nTimes); return this; }

		//}

		//public static KeysToSend K { get => new KeysToSend(); }


		static void Test()
		{
			//Key("text", K.Ctrl.A.Tab.Execute(9).Enter);
		}
	}
}
