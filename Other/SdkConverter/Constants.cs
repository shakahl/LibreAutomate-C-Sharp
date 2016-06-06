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
				if(!_defineConst.Remove(name)) _defineOther.Remove(name);
				//Out($"#undef {name}");
			} else if(iValue < iNext) { //preprocessor removes some #define values, it's ok
				bool notConst; string type;
				string value = _ConstValue(iParamOrValue, iNext, out notConst, out type);
				//info: for func-style get parameters as part of value

				//OutList(name, value); //TODO: some values sould be removed, now not. Should be `name(x) = name(x)', ie the right part unexpanded.

				if(isFunc) {
					_defineOther[name] = value;
				} else if(value!= null) {
					__sbDef.Clear();
					__sbDef.AppendFormat("public {2} {3} {0} = {1};", name, value, notConst ? "readonly" : "const", type);
					_defineConst[name] = __sbDef.ToString();
				} else {
					_defineConst.Remove(name);
					//TODO: add commented
                }
			}

			_i = iNext - 1;
		}

		StringBuilder __sbDef = new StringBuilder();

		/// <summary>
		/// Converts one or more tokens to C# constant value.
		/// Returns null if cannot convert.
		/// </summary>
		/// <param name="iTokFrom">The first token.</param>
		/// <param name="iTokTo">Token after the last token.</param>
		/// <param name="notConst">Declaration keyword to use: false - const, true - readonly.</param>
		/// <param name="type">Constant type: "int", "uint", "string" etc.</param>
		string _ConstValue(int iTokFrom, int iTokTo, out bool notConst, out string type)
		{
			//char* s = T(iTokFrom);
			//iTokTo--;
			//return new string(s, 0, (int)(T(iTokTo) - s) + _tok[iTokTo].len);

			type = "int";
			notConst = false;

			__sbDef.Clear();
			bool isPrevTokIdentNumberString = false;
			string sPrev = null;
			for(int i = iTokFrom; i < iTokTo; i++) {
				string s = null;

				//unalias, unenclose, resolve sizeof if possible
				if(i <= iTokTo - 3 && _TokIsChar(i, '(') && _TokIsChar(i + 2, ')')) {
					if(_TokIsNumber(i + 1)) { //unenclose number
						s = _TokToString(i + 1);
						i += 2;
					} else if(_TokIsIdent(i + 1)) { //unalias
						s = __ConstValue_UnaliasAndSizeof(iTokFrom, ref i);
                    }
				}
				if(s == null) s = _TokToString(i);

				//add unchecked
				//if(s.Length == 10 && s.StartsWith_("0x") && s[2] >= '8') {
				//	Out(sPrev);
				//	if(sPrev == "(int)") __sbDef.Insert(__sbDef.Length - 5, " unchecked(");
				//	else __sbDef.Append(" unchecked((int)");
				//	s += ")";

				//	//TODO: remove "(uint)" cast
				//}

				//spaces
				char c = s[0];
				bool isTokIdentNumberString = _IsCharIdent(c) || c == '\"' || c == '\'';
				if(isPrevTokIdentNumberString && isTokIdentNumberString) __sbDef.Append(' ');
				isPrevTokIdentNumberString = isTokIdentNumberString;

				__sbDef.Append(s);
				sPrev = s;
			}

			//TODO: unchecked

			return __sbDef.ToString();

			//rules:
			//If first cast is uint/long/ulong, let const type be uint/long/ulong.
			//If a number has suffix U/L/UL, let const type be uint/long/ulong.
			//If first cast is IntLong, let const type be readonly IntLong.
			//Convert (uint)~0 to 0xffffffff.
		}

		string __ConstValue_UnaliasAndSizeof(int iTokFrom, ref int iCurrent)
		{
			string s = null;
			int i = iCurrent;
			_Symbol x;
			if(_TryFindSymbol(i + 1, out x, true) && !(x is _Keyword)) {
				int ptr = _Unalias(i + 1, ref x);
				s = x.csTypename;

				bool convertedSizeof = false;
				if(i > iTokFrom && _TokIs(i - 1, "sizeof")) {
					bool failed = false;
					if(ptr != 0) {
						ptr = 0;
						s = "IntPtr.Size";
					} else if(x is _CppType) {
						s = (x as _CppType).sizeBytesCpp.ToString();
					} else if(x is _Callback) {
						s = "IntPtr.Size";
					} else {
						switch(s) {
						case "IntLong":
							s = "IntPtr.Size";
							break;
						case "SID":
							s = "12";
							break;
						case "WSACMSGHDR":
							s = "IntPtr.Size+8";
							break;
						default:
							failed = true;
							break;
						}
					}

					//OutList(_TokToString(i + 1), s);
					if(!failed) {
						__sbDef.Remove(__sbDef.Length - 6, 6); //remove "sizeof"
						convertedSizeof = true;
					}
				}

				if(!convertedSizeof) {
					s = "(" + s + ")";
					while(ptr-- > 0) s += "*";
				}
				i += 2;
			}

			iCurrent = i;
            return s;
		}
	}
}
