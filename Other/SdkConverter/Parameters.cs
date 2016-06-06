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
			//TODO: don't use this func. Now used in pragma.

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
					_SkipEnclosed();
					break;
				}
			}
		}

		/// <summary>
		/// Converts function parameters to C# and appends to sb like "(...);\r\n".
		/// _i must be at '('. Finally will be after ')'.
		/// </summary>
		void _ConvertParameters(StringBuilder sb, string parentName)
		{
			if(!_TokIsChar(_i, '(')) _Err(_i, "unexpected");
			_i++;
			sb.Append('(');

			if(!_TokIsChar(_i, ')')) {
				for(int iParam = 0; ; iParam++, _i++) {
					_PARAMDATA t;
					_ParseParamOrMember(false, out t, parentName, iParam + 1);

					//skip default value of optional parameter
					if(_TokIsChar(_i, '=')) {
						while(!_TokIsChar(++_i, ',', ')')) { }
					}

					//is ',' or ')'?
					bool isLast = _TokIsChar(_i, ')');
					if(!isLast && !_TokIsChar(_i, ',')) _Err(_i, "unexpected");

					sb.AppendFormat("{0}{1} {2}{3}", t.attributes, t.typeName, t.name, isLast ? "" : ", ");

					if(isLast) break;
				}
			}
			_i++; //skip ')'
			sb.Append(");\r\n");
		}

		struct _PARAMDATA
		{
			public string typeName; //C# typename
			public string name; //parameter name
			public string attributes; //C# attributes
			public bool isNestedTypeDefinitionWithoutVariables;
			public bool isAnonymousTypeDefinitionWithoutVariables;
		}

		/// <summary>
		/// Converts C++ function parameter or struct member definition (type, name, const etc) to C# typename/name/etc.
		/// _i must be at its start. Finally it will be after the parsed part. Does not check whether it is followed by ',' etc.
		/// </summary>
		/// <param name="isMember">false if parameter, true if member.</param>
		/// <param name="t">Receives C# typename, name etc.</param>
		/// <param name="parentName">The function/struct name. Used to auto-create names for nameless parameters.</param>
		/// <param name="iParam">1-based index of parameter/member. Used to auto-create names for nameless parameters.</param>
		void _ParseParamOrMember(bool isMember, out _PARAMDATA t, string parentName, int iParam)
		{
			t = new _PARAMDATA();

			var d = new _FINDTYPEDATA();
			_FindTypename(isMember, ref d);
			if(d.outIsNestedTypeDefinition) {
				if(_TokIsChar(_i, ';')) {
					t.isNestedTypeDefinitionWithoutVariables = true;
					t.isAnonymousTypeDefinitionWithoutVariables = d.outIsAnonymousTypeDefinition;
					t.typeName = d.outSym.csTypename;
					return;
				}
			} else _i++;

			//pointer
			int ptr = 0;
			while(_TokIsChar(_i, '*', '&')) { _i++; ptr++; }

			//suport inline function type definition
			bool isFunc = !d.outIsNestedTypeDefinition && _DetectIsFuncType(_i);
			if(isFunc) {
				var f = new _INLINEFUNCTYPEDATA(parentName, iParam);
				_DeclareTypedefFunc(d.outSym, ptr, d.outIsConst, d.outTypenameToken, f);
				t.typeName = f.typeName;
				t.name = f.paramName;
			} else {
				//typename, param/member name
				t.typeName = _ConvertTypeName(d.outSym, ref ptr, !isMember, d.outIsConst, d.outTypenameToken);

				if(_TokIsIdent(_i)) t.name = _TokToString(_i++);
				else if(!isMember && _TokIsChar(_i, ",)[=")) t.name = "param" + iParam;
				else _Err(_i, "no name");
			}

			if(_TokIsChar(_i, '[')) {
				t.attributes = _CArrayToMarshalAsAttribute(!isMember);
				t.typeName += "[]";
			}
		}

		struct _FINDTYPEDATA
		{
			public _Symbol outSym;
			public bool outIsConst;
			public bool outIsAnonymousTypeDefinition;
			public bool outIsNestedTypeDefinition;
			public int outTypenameToken;
			public int inTypedefNameToken;
		}

		/// <summary>
		/// Finds typename. Gets its _Symbol etc.
		/// Starts searching from _i. Finally _i will be typename token index.
		/// Error if not found.
		/// Initially sets all outX=0/false/null.
		/// If there is 'const', sets outIsConst=true.
		/// If there is 'struct' etc as nested definition, converts the definition, sets outIsNestedStruct=true; then _i finally will be after }, which can be semicolon, variable name or * variable name.
		/// If there is 'struct' etc as forward declaration, adds forward declaration.
		/// </summary>
		void _FindTypename(bool isMember, ref _FINDTYPEDATA d)
		{
			d.outSym = null;
			d.outIsConst = false;
			d.outIsNestedTypeDefinition = false;
			d.outTypenameToken = 0;

			//process keywords until typename
			for(; _TokIsIdent(_i); _i++) {
				d.outSym = _FindSymbol(_i, true);
				var k = d.outSym as _Keyword;
				if(k == null) {
					d.outTypenameToken = _i;
					return;
				}

				switch(k.kwType) {
				case _KeywordT.TypeDecl:
					//is nested struct/enum/typedef definition?
					if(isMember) {
						bool isDef = false;
						if(_TokIsChar(_i, 't')) isDef = true;
						else {
							int j = _i + 1; if(_TokIsIdent(j)) j++;
							if(_TokIsChar(j, "{:;")) isDef = true;
						}
						if(isDef) {
							_DeclareType(ref d);
							d.outIsNestedTypeDefinition = true;
							return;
						}
					}

					//inline forward declaration
					d.outSym = _InlineForwardDeclaration();
					d.outTypenameToken = _i;
					return;
				case _KeywordT.Normal:
					if(_TokIs(_i, "const")) { d.outIsConst = true; continue; }
					break;
				}

				_Err(_i, "unexpected");
			}
			_Err(_i, "no type");
		}

		/// <summary>
		/// Converts type name/pointer to C# type name/ref/pointer.
		/// </summary>
		/// <param name="t">The type.</param>
		/// <param name="ptr">Pointer level. The function may adjust it depending on typedef pointer level etc.</param>
		/// <param name="useRefOut">Convert * to ref/out.</param>
		/// <param name="isConst">Is const.</param>
		/// <param name="iTokTypename">Type name token index. Can be 0 if don't need to unalias etc; then just adds pointer/ref if need.</param>
		string _ConvertTypeName(_Symbol t, ref int ptr, bool useRefOut, bool isConst, int iTokTypename)
		{
			string name;
			bool isLangType = false;

			if(iTokTypename != 0) {
				ptr += _Unalias(iTokTypename, ref t);
				name = t.csTypename;
				var c = t as _CppType;
				if(c != null) {
					isLangType = true;
					if(ptr != 0) {
						if(name == "char") {
							ptr--;
							name = (ptr == 0 && isConst) ? "string" : "StringBuilder";
							isConst = false;
							//TODO: typedef also can be const or not, eg LPWCSTR or LPWSTR
						}
					}
				}
			} else name = t.csTypename;

			if(isLangType) {
				if(ptr > 0 && name == "void") {
					name = "IntPtr";
					ptr--;
				}
			} else {
				var ts = t as _Struct;
				if(ts != null) {
					if(ts.isInterface) {
						//OutList(ptr, ts.csTypename);
						if(ptr == 0) _Err(iTokTypename, "interface and not pointer");
						ptr--;
					} else if(!ts.isClass) {
						switch(name) {
						case "GUID": name = "Guid"; break;
						case "VARIANT": name = "object"; break;
						//case "SAFEARRAY": //in SDK used only with SafeArrayX functions, with several other not-important functions and as [PROP]VARIANT members
							//Out(ptr);
							//break;
						}
					}
				}

				//if(t.forwardDecl) {
				//	OutList(name, iTokTypename);
				//}
			}

			if(ptr == 0) return name;

			__ctn.Clear();
			if(useRefOut) {
				ptr--;
				//__ctn.Append(isConst ? "ref " : "out ");
				__ctn.Append("ref ");
			}
			__ctn.Append(name);
			__ctn.Append('*', ptr);

			return __ctn.ToString();
		}
		StringBuilder __ctn = new StringBuilder();

		/// <summary>
		/// Converts "[x]" to "[MarshalAs(UnmanagedType.ByValArray, SizeConst = {x})]".
		/// Supports "[y][x]" etc.
		/// _i must be at '['. Finally it will be after ']'.
		/// If empty array (like TYPE x[]), returns "//...".
		/// If justSkipEnclosed, just skips [..] or [...][...] and returns null; it can be used for parameters, because C++ arguments for parameters 'TYPE param[n]' are passed like 'TYPE* param', and n is ignored, can be empty.
		/// </summary>
		string _CArrayToMarshalAsAttribute(bool justSkipEnclosed = false)
		{
			string elemCount = null, comment = null;
			for(; _TokIsChar(_i, '['); _i++) { //support [a][b]
				int i0 = _i + 1;
				_SkipEnclosed();
				if(justSkipEnclosed) continue;
				bool notConst; string type, ec = _ConstValue(i0, _i, out notConst, out type);
				if(ec.Length == 0) ec = "0"; //TYPE x[]
				if(ec == "0") comment = "//";
				if(elemCount == null) elemCount = ec; else elemCount = $"({elemCount})*({ec})";
			}
			if(justSkipEnclosed) return null;
			return $"{comment}[MarshalAs(UnmanagedType.ByValArray, SizeConst = {elemCount})]";
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
