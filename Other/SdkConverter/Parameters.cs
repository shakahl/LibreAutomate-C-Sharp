using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Windows.Forms;
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

namespace SdkConverter
{
	unsafe partial class Converter
	{

		struct _PARAMETER
		{
			internal int iTok;
			internal int nTok;
			internal _PARAMETER(int iTok, int nTok) { this.iTok = iTok; this.nTok = nTok; }
		}

		/// <summary>
		/// Used with all non-nested _GetParameters().
		/// </summary>
		List<_PARAMETER> _params = new List<_PARAMETER>();

		/// <summary>
		/// Parses parameters and stores their iTok/nTok in p.
		/// Error if _i is not at the starting '('. Finally _i will be at the ending ')'.
		/// Returns the number of parameters.
		/// </summary>
		[DebuggerStepThrough]
		int _GetParameters(List<_PARAMETER> p)
		{
			p.Clear();

			char* s = T(_i);
			if(*s != '(') _Err(s, "expected (");

			for(int paramStart = 0, iVeryStart = _i++; ; _i++) {
				s = T(_i);
				char c = *s;
				switch(c) {
				case ',':
				case ')':
					if(paramStart == 0) {
						if(c == ')' && p.Count == 0) return 0; //()
						_Err(s, "unexpected");
					}
					p.Add(new _PARAMETER(paramStart, _i - paramStart));
					if(c == ')') return p.Count;
					paramStart = 0;
					continue;
				case '\0':
					_Err(iVeryStart, "no )");
					break;
				case ';':
					_Err(s, "unexpected");
					break;
				}

				if(paramStart == 0) paramStart = _i;

				switch(c) {
				case '(':
				case '[':
				//case '<': //can be operator
				case '{':
					_i = _SkipEnclosed(_i);
					break;
				}
			}
		}

		/// <summary>
		/// Converts function parameters to C# and appends to sb like "(...);\r\n".
		/// </summary>
		/// <param name="isUnsafe">If parameters have pointers or unsafe types, sets=true, else does not change.</param>
		void _ConvertParameters(List<_PARAMETER> p, ref StringBuilder sb, ref bool isUnsafe, string parentName)
		{
			sb.Append('(');
			_INLINEFUNCTYPEDATA f = null;

			for(int iParam = 0; iParam < p.Count; iParam++) {
				int i = p[iParam].iTok, iTo = i + p[iParam].nTok;

				_PARAMDATA t;
				i = _ParseParamOrMember(false, i, out t, ref f, parentName, iParam);
				Debug.Assert(i <= iTo);
				if(i != iTo) _Err(i, "unexpected");

				if(t.isUnsafe) isUnsafe = true;
				//TODO: also set isUnsafe if some parameters are unsafe types.

				sb.AppendFormat("{0}{1} {2}{3}", t.attributes, t.typeName, t.name, (iParam < p.Count - 1) ? ", " : "");
			}

			sb.Append(");\r\n");
			if(f != null) sb.Append(f.sb.ToString());
		}

		struct _PARAMDATA
		{
			//public _Symbol t;
			public string typeName;
			public string name;
			public string attributes;
			public bool isUnsafe;
		}

		/// <summary>
		/// Converts C++ function parameter or struct member definition (type, name, const etc) to C# typename/name/etc.
		/// Returns token index after the parsed part. Does not check whether it is ',' etc.
		/// </summary>
		/// <param name="isMember">false if parameter, true if member.</param>
		/// <param name="i">Starting token index.</param>
		/// <param name="t">Receives C# typename, name etc.</param>
		/// <param name="f">An _INLINEFUNCTYPEDATA variable common to all parameters/members of the function/struct. Caller should just declare it and set = null in the declaration. Finally, if not null, its .sb may contain delegate definitions; append it to _sbDelegate.</param>
		/// <param name="parentName">The function/struct name. Used to auto-create names for nameless parameters.</param>
		/// <param name="iParam">0-based index of parameter/member. Used to auto-create names for nameless parameters.</param>
		int _ParseParamOrMember(bool isMember, int i, out _PARAMDATA t, ref _INLINEFUNCTYPEDATA f, string parentName, int iParam)
		{
			t = new _PARAMDATA();
			_Symbol x = null;
			int ptr = 0;
			bool isConst = false;

			//process keywords until typename
			for(; _TokIsIdent(i); i++) {
				x = _FindSymbol(i);
				var k = x as _Keyword;
				if(k == null) goto gFoundTypename;

				switch(k.kwType) {
				case _KeywordT.TypeDecl: //inline forward declaration (keyword 'struct' etc)
					if(*T(i++) == 'e') {
						if(!(_TryFindSymbol(i, out x) && x is _Enum)) _AddSymbol(i, x = new _Enum(true));
					} else {
						if(!(_TryFindSymbol(i, out x) && x is _Struct)) _AddSymbol(i, x = new _Struct(true));
					}
					goto gFoundTypename;
				case _KeywordT.Normal:
					if(_TokIs(i, "const")) { isConst = true; continue; }
					break;
				}

				_Err(i, "not impl: param modifier"); //TODO: rem
				_Err(i, "unexpected");
			}
			_Err(i, "no type");

			gFoundTypename:
			int iTypeName = i++;

			//pointer
			while(_TokIsChar(i, '*', '&')) { i++; ptr++; }

			//suport inline function type definition
			bool isFunc = _DetectIsFuncType(i);
			if(isFunc) {
				if(f == null) f = new _INLINEFUNCTYPEDATA(parentName);
				f.paramIndex = iParam + 1;
				int save_i = _i; _i = i;
				_DeclareTypedefFunc(iTypeName, x, ptr, isConst, f);
				i = _i; _i = save_i;
				t.typeName = f.typeName;
				t.name = f.paramName;
				if(f.isUnsafe) t.isUnsafe = true;
			}

			//typename, param/member name
			if(!isFunc) {
				t.typeName = _ConvertTypeName(iTypeName, x, ref ptr, !isMember, isConst);
				if(ptr != 0) t.isUnsafe = true;

				if(_TokIsIdent(i)) t.name = _tok[i++].ToString();
				else if(!isMember && _TokIsChar(i, ',', ')')) t.name = "param" + (iParam + 1);
				else _Err(i, "no name");
			}

			if(_TokIsChar(i, '[')) {
				i = _CArrayToMarshalAsAttribute(i, out t.attributes);
				t.typeName += "[]";
			}

			return i;
		}

		/// <summary>
		/// Converts type name/pointer to C# type name/ref/out/pointer.
		/// </summary>
		/// <param name="iTokName">Type name token index.</param>
		/// <param name="t">Can be null, eg if using forward-declared type.</param>
		/// <param name="ptr">Pointer level. The function may adjust it depending on typedef pointer level etc.</param>
		/// <param name="useRefOut">Convert * to ref/out.</param>
		/// <param name="isConst">Is const.</param>
		string _ConvertTypeName(int iTokName, _Symbol t, ref int ptr, bool useRefOut, bool isConst)
		{
			string name = null;

			if(t != null) {
				ptr += _Unalias(iTokName, ref t);
				var c = t as _CppType;
				if(c != null) {
					name = c.csType;

					if(ptr != 0) {
						if(name == "char") {
							ptr--;
							name = (ptr == 0 && isConst) ? "string" : "StringBuilder";
							isConst = false;
							//TODO: typedef also can be const or not, eg LPWCSTR or LPWSTR
						}
					}
				}
			}

			if(name == null) name = _tok[iTokName].ToString();

			if(ptr == 0) return name;

			__ctn.Clear();
			if(useRefOut) {
				ptr--;
				__ctn.Append(isConst ? "ref " : "out ");
			}
			__ctn.Append(name);
			__ctn.Append('*', ptr);

			return __ctn.ToString();
		}
		StringBuilder __ctn = new StringBuilder();

		/// <summary>
		/// Converts "[x]" to "[MarshalAs(UnmanagedType.ByValArray, SizeConst = {x})]".
		/// Supports "[y][x]" etc.
		/// i must be token index at '['. Returns token index after ']'.
		/// </summary>
		int _CArrayToMarshalAsAttribute(int i, out string s)
		{
			string elemCount = null;
			while(_TokIsChar(i, '[')) { //support [a][b]
				char* t = T(i + 1);
				i = _SkipEnclosed(i);
				//TODO: convert sizeof(DWORD) to sizeof(int) etc
				string ec = new string(t, 0, (int)(T(i++) - t));
				if(elemCount == null) elemCount = ec; else elemCount = $"({elemCount})*({ec})";
			}
			s = $"[MarshalAs(UnmanagedType.ByValArray, SizeConst = {elemCount})]";
			return i;
		}

		/// <summary>
		/// Converts C++ calling convention keyword to C# enum CallingConvention member name.
		/// Returns null if __stdcall.
		/// </summary>
		/// <param name="iTok">C++ calling convention keyword token. Error if other token.</param>
		string _ConvertCallConv(int iTok)
		{
			_Keyword cc = _FindKeyword(iTok);
			if(cc.kwType != _KeywordT.CallConv) _Err(iTok, "unexpected");

			char* s = T(iTok);
			while(*s == '_') s++;
			switch(*s) {
			case 'c': return "Cdecl";
			case 'f': return "FastCall";
			case 't': return "ThisCall";
			}
			return null; //StdCall is default
		}
	}
}
