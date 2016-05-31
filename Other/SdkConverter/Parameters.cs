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
			public int iTok;
			public int nTok;
			public _PARAMETER(int iTok, int nTok) { this.iTok = iTok; this.nTok = nTok; }
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
				i = _ParseParamOrMember(false, i, out t, ref f, parentName, iParam + 1);
				Debug.Assert(i <= iTo);
				//string optParamValue = null;
				if(i != iTo) {
					if(!_TokIsChar(i, '=')) _Err(i, "unexpected");
					//default value for optional parameter
					//optParamValue = new string(T(i), 0, (int)(T(iTo - 1) + _tok[iTo - 1].len - T(i))); //this works, bt gets raw value, converting can be dificult, SDK does not have such things
					i = iTo;
				}

				if(t.isUnsafe) isUnsafe = true;
				//TODO: also set isUnsafe if some parameters are unsafe types.

				sb.AppendFormat("{0}{1} {2}{3}", t.attributes, t.typeName, t.name, (iParam < p.Count - 1) ? ", " : "");
				//sb.AppendFormat("{0}{1} {2}{3}{4}", t.attributes, t.typeName, t.name, optParamValue,(iParam < p.Count - 1) ? ", " : "");
			}

			sb.Append(");\r\n");
			if(f != null) sb.Append(f.sb.ToString());
		}

		struct _PARAMDATA
		{
			public string typeName; //C# typename
			public string name; //parameter name
			public string attributes; //C# attributes
			public bool isUnsafe; //has pointers etc (need to declare parent as unsafe)
			public bool isNestedTypeDefinitionWithoutVariables;
			public bool isAnonymousTypeDefinitionWithoutVariables;
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
		/// <param name="iParam">1-based index of parameter/member. Used to auto-create names for nameless parameters.</param>
		int _ParseParamOrMember(bool isMember, int i, out _PARAMDATA t, ref _INLINEFUNCTYPEDATA f, string parentName, int iParam)
		{
			t = new _PARAMDATA();

			var d = new _FINDTYPEDATA();
			i = _FindTypename(isMember, i, ref d);
			if(d.outIsNestedTypeDefinition) {
				if(_TokIsChar(i, ';')) {
					t.isNestedTypeDefinitionWithoutVariables = true;
					t.isAnonymousTypeDefinitionWithoutVariables = d.outIsAnonymousTypeDefinition;
					t.typeName = d.outTypename;
					return i;
				}
			} else i++;

			//pointer
			int ptr = 0;
			while(_TokIsChar(i, '*', '&')) { i++; ptr++; }

			//suport inline function type definition
			bool isFunc = _DetectIsFuncType(i);
			if(isFunc) {
				if(f == null) f = new _INLINEFUNCTYPEDATA(parentName);
				f.paramIndex = iParam;
				int save_i = _i; _i = i;
				_DeclareTypedefFunc(d.outSym, ptr, d.outIsConst, d.outTypenameToken, d.outTypename, f);
				i = _i; _i = save_i;
				t.typeName = f.typeName;
				t.name = f.paramName;
				if(f.isUnsafe) t.isUnsafe = true;
			} else {
				//typename, param/member name
				t.typeName = _ConvertTypeName(d.outSym, ref ptr, !isMember, d.outIsConst, d.outTypenameToken, d.outTypename);
				if(ptr != 0) t.isUnsafe = true;

				if(_TokIsIdent(i)) t.name = _TokToString(i++);
				else if(!isMember && _TokIsChar(i, ',', ')', '[', '=')) t.name = "param" + iParam;
				else _Err(i, "no name");
			}

			if(_TokIsChar(i, '[')) {
				i = _CArrayToMarshalAsAttribute(i, out t.attributes);
				t.typeName += "[]";
			}

			return i;
		}

		struct _FINDTYPEDATA
		{
			public _Symbol outSym;
			public bool outIsConst;
			public bool outIsAnonymousTypeDefinition;
			public bool outIsNestedTypeDefinition;
			public int outTypenameToken;
			public string outTypename;
		}

		/// <summary>
		/// Finds and returns typename token index. Gets its _Symbol etc.
		/// Starts searching from i.
		/// Error if not found.
		/// Initially sets all outX=0/false/null.
		/// If there is 'const', sets outIsConst=true.
		/// If there is 'struct' etc as nested definition, converts the definition, sets outIsNestedStruct=true and sets outName to its type name; then returns token index after }, which can be semicolon, variable name or * variable name.
		/// Else sets outTypenameToken.
		/// If there is 'struct' etc as forward declaration, adds forward declaration.
		/// </summary>
		int _FindTypename(bool isMember, int i, ref _FINDTYPEDATA d)
		{
			d.outSym = null;
			d.outIsConst = false;
			d.outIsNestedTypeDefinition = false;
			d.outTypenameToken = 0;
			d.outTypename = null;

			//process keywords until typename
			for(; _TokIsIdent(i); i++) {
				d.outSym = _FindSymbol(i, true);
				var k = d.outSym as _Keyword;
				if(k == null) {
					d.outTypenameToken = i;
					return i;
				}

				switch(k.kwType) {
				case _KeywordT.TypeDecl: //keyword 'struct' etc
					//is nested struct/enum/typedef definition?
					if(isMember) {
						bool isDef = false;
						if(_TokIsChar(i, 't')) isDef = true;
						else {
							int j = i + 1; if(_TokIsIdent(j)) j++;
							if(_TokIsChar(j, '{', ':', ';')) isDef = true;
						}
						if(isDef) {
							int save_i = _i; _i = i;
							_DeclareType(true, ref d);
							i = _i; _i = save_i;
							d.outIsNestedTypeDefinition = true;
							return i;
						}
					}

					//inline forward declaration
					if(*T(i++) == 'e') {
						if(!(_TryFindSymbol(i, out d.outSym, true) && d.outSym is _Enum)) _AddSymbol(i, d.outSym = new _Enum(true), true);
					} else {
						if(!(_TryFindSymbol(i, out d.outSym, true) && d.outSym is _Struct)) _AddSymbol(i, d.outSym = new _Struct(true), true);
					}
					d.outTypenameToken = i;
					return i;
				case _KeywordT.Normal:
					if(_TokIs(i, "const")) { d.outIsConst = true; continue; }
					break;
				}

				_Err(i, "unexpected");
			}
			_Err(i, "no type");
			return 0;
		}

		/// <summary>
		/// Converts type name/pointer to C# type name/ref/out/pointer.
		/// </summary>
		/// <param name="t">Can be null, eg if using forward-declared type.</param>
		/// <param name="ptr">Pointer level. The function may adjust it depending on typedef pointer level etc.</param>
		/// <param name="useRefOut">Convert * to ref/out.</param>
		/// <param name="isConst">Is const.</param>
		/// <param name="iTokTypename">Type name token index. Not used if nestedStructName not null.</param>
		/// <param name="nestedStructName">Use when the type is now defined nested struct/enum.</param>
		string _ConvertTypeName(_Symbol t, ref int ptr, bool useRefOut, bool isConst, int iTokTypename, string nestedStructName = null)
		{
			string name = nestedStructName;

			if(t != null && nestedStructName == null) {
				ptr += _Unalias(iTokTypename, ref t);
				var c = t as _CppType;
				if(c != null) {
					name = c.cs;

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

			if(name == null) name = _TokToString(iTokTypename);

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
			_FindKeyword(iTok, _KeywordT.CallConv);

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
