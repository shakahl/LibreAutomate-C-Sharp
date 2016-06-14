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
using System.ComponentModel; //Win32Exception

//using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace SdkConverter
{
	unsafe partial class Converter
	{
		void _DefineUndef()
		{
			char* s = T(++_i);
			char c = *s; //was like `d$$$_REALNAME, now s is without `
			s += 5; int lenName = _tok[_i].len - 5; //skip prefix 'd$$$_' that was added to avoid unexpanding names
													//_tok[_i] = new _Token(s, lenName);

			//is function-style?
			char* s2 = T(++_i);
			bool isFunc = c == 'd' && *s2 == '(' && s2 == s + lenName;

			//find value
			int iValue = _i, iParamOrValue = _i;
			if(isFunc) iValue = _SkipEnclosed(_i) + 1; //name(parameters)[ value]

			//find next line
			int iNext = iValue;
			for(; ; iNext++) {
				char k = *T(iNext);
				if(k == '`' || k == '\x0') break;
			}

			string name = new string(s, 0, lenName);
			if(c == 'u') { //#undef
				if(!_defineConst.Remove(name) && !_defineOther.Remove(name)) _defineW.Remove(name);
				//Out($"#undef {name}");
			} else if(iValue < iNext) { //preprocessor removes some #define values, it's ok
				if(isFunc) { //info: for func-style get parameters as part of value
					_defineOther[name] = _TokToString(iParamOrValue, iNext);
				} else {
					_ExpressionResult r = _Expression(iParamOrValue, iNext, name);
					if(r.typeS == null) {
						bool isFuncWA = (iNext - iValue == 1) && __DefineWA(name, r.valueS);
						if(!isFuncWA) _defineOther[name] = " " + r.valueS;
					} else {
						__sbDef.Clear();
						__sbDef.AppendFormat("public {2} {3} {0} = {1};", name, r.valueS, r.notConst ? "readonly" : "const", r.typeS);
						_defineConst[name] = __sbDef.ToString();
					}
				}
			}

			_i = iNext - 1;
		}

		StringBuilder __sbDef = new StringBuilder();

		bool __DefineWA(string name, string value)
		{
			int suffixLen = 0;
			if(value.Length == name.Length + 1 && value.EndsWith_("W")) suffixLen = 1;
			else if(value.Length == name.Length + 2 && value.EndsWith_("_W")) suffixLen = 2; //some struct
			if(!(suffixLen > 0 && value.StartsWith_(name))) {
				//Out($"<><c 0xff>{name}    {value}</c>");
				return false;
			}

			string def;
			if(suffixLen==1 && _func.TryGetValue(value, out def)) {
				//if(!_func.Remove(name + "A")) Out($"<><c 0xff>{name}    {value}</c>"); //about 20 in SDK don't not have A versions
				_func.Remove(name + "A");

				def = def.Replace("W(", "(");

				if(!def.Contains(", EntryPoint=\"")) {
					def = def.Replace(")]", ", EntryPoint=\"" + value + "\")]");
				}

				_func[value] = def;

				//Out($"<><c 0xff0000>{name}    {value}</c>");
				//Out(def);
				return true;
			} else {
				_Symbol x;
				if(_ns[0].sym.TryGetValue(_TokenFromString(value), out x)) {
					var t = x as _Struct;
					if(t != null || x is _Callback) {
						//if(x is _Struct) Out($"<><c 0xff0000>{name}    {value}</c>");
						//else Out($"<><c 0x8000>{name}    {value}</c>");
						int v =(t == null) ? 2 : (t.isInterface ? 1 : 0);
						if(suffixLen == 2) v |= 0x10000;
						_defineW[name] = v; //later will replace all STRUCTW to STRUCT in the final string of struct/func/delegate/interface
						return true;
					}
					//else if(x is _Typedef) {
					//	Out($"<><c 0x80>{name}    {value}</c>");
					//} else {
					//	Out($"<><c 0xFF>{name}    {value}</c>"); //0
					//}

				} else {
					//Out($"<><c 0xff>{name}    {value}</c>");

				}
			}

			return false;
		}
	}
}
