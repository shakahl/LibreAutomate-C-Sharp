using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
		/// Currently used only with pragma.
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
					_SkipEnclosed();
					break;
				}
			}
		}

		/// <summary>
		/// Converts function parameters to C# and appends to sb like "(...);\r\n".
		/// _i must be at '('. Finally will be after ')'.
		/// </summary>
		void _ConvertParameters(StringBuilder sb, string parentName, _TypeContext context)
		{
			if(!_TokIsChar(_i, '(')) _Err(_i, "unexpected");
			_i++;
			sb.Append('(');

			if(!_TokIsChar(_i, ')')) {
				for(int iParam = 0; ; iParam++, _i++) {

					//ignore ...
					if(_TokIsChar(_i, '.') && _TokIsChar(_i + 1, '.') && _TokIsChar(_i + 2, '.') && _TokIsChar(_i + 3, ')')) {
						_i += 3;
						sb.Remove(sb.Length - 2, 2); //", "
						break;
					}

					_PARAMDATA t;
					_ParseParamOrMember(context, out t, parentName, iParam + 1);

					//skip default value of optional parameter
					if(_TokIsChar(_i, '=')) {
						while(!_TokIsChar(++_i, ',', ')')) { }
					}

					//is ',' or ')'?
					bool isLast = _TokIsChar(_i, ')');
					if(!isLast && !_TokIsChar(_i, ',')) _Err(_i, "unexpected");

					if(t.attributes != null) { sb.Append(t.attributes); sb.Append(' '); }
					sb.Append(t.typeName); sb.Append(' '); sb.Append(t.name);
					if(!isLast) sb.Append(", ");

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
		/// <param name="context">Member, Parameter or DelegateParameter.</param>
		/// <param name="t">Receives C# typename, name etc.</param>
		/// <param name="parentName">The function/struct name. Used to auto-create names for nameless parameters.</param>
		/// <param name="iParam">1-based index of parameter/member. Used to auto-create names for nameless parameters.</param>
		void _ParseParamOrMember(_TypeContext context, out _PARAMDATA t, string parentName, int iParam)
		{
			bool isMember = context == _TypeContext.Member;

			t = new _PARAMDATA();

			int iSAL = 0;
			if(_TokIsChar(_i, '^') && _TokIsChar(_i + 1, '\"')) {
				_i++;
				if(!isMember) {
					iSAL = _i;
					_tok[iSAL] = new _Token(_tok[iSAL].s + 1, _tok[iSAL].len - 2);
				}
				_i++;
			}

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
				t.typeName = _ConvertTypeName(d.outSym, ref ptr, d.outIsConst, d.outTypenameToken, context, out t.attributes, iSAL);

				if(_TokIsIdent(_i)) t.name = _TokToString(_i++);
				else if(!isMember && _TokIsChar(_i, ",)[=")) t.name = "param" + iParam;
				else _Err(_i, "no name");
			}

			if(_TokIsChar(_i, '[')) {
				_ConvertCArray(ref t.typeName, ref t.name, ref t.attributes, !isMember);
			}

			//escape names that are C# keywords
			if(_csKeywords.Contains(t.name)) {
				//Print(t.name);
				t.name = "@" + t.name;
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

		enum _TypeContext
		{
			Parameter, DelegateParameter, Return, Member, ComParameter, ComReturn
		}

		/// <summary>
		/// Converts type name/pointer to C# type name.
		/// The return value is type name, possibly with * or []. If parameter, also can have ref/out and [In]/[Out].
		/// </summary>
		/// <param name="t">The type.</param>
		/// <param name="ptr">Pointer level. The function may adjust it depending on typedef pointer level etc.</param>
		/// <param name="isConst">Is const.</param>
		/// <param name="iTokTypename">Type name token index. Can be 0 if don't need to unalias etc; then just adds pointer/ref if need.</param>
		/// <param name="context">Depending on it char* will be converted to "string", "StringBuilder" etc.</param>
		/// <param name="attributes">Receives null or MarshalAs attribute.</param>
		/// <param name="iSAL">SAL token index, or 0 if no SAL.</param>
		string _ConvertTypeName(_Symbol t, ref int ptr, bool isConst, int iTokTypename, _TypeContext context, out string attributes, int iSAL = 0)
		{
			attributes = null;
			string name, marshalAs = null;

			bool isLangType = false, isParameter = false, isCOM = false, isBlittable = false;
			switch(context) {
			case _TypeContext.Parameter: case _TypeContext.DelegateParameter: isParameter = true; break;
			case _TypeContext.ComParameter: isParameter = true; isCOM = true; break;
			case _TypeContext.ComReturn: isCOM = true; break;
			}

			bool _In_ = false, _Out_ = false, _Inout_ = false;
			if(iSAL > 0) {
				//Print(_tok[iSAL]);
				if(_TokStarts(iSAL, "_In_")) _In_ = true;
				else if(_TokStarts(iSAL, "_Out")) _Out_ = true;
				else if(_TokStarts(iSAL, "_Inout_")) _Inout_ = true;
				else _Err(iSAL, "unknown SAL");
			}

			if(iTokTypename != 0) {
				ptr += _Unalias(iTokTypename, ref t, ref isConst);
				name = t.csTypename;

				var c = t as _CppType;
				if(c != null) {
					isLangType = true;
					isBlittable = true;
					switch(name) {
					case "char":
						isBlittable = false;
						if(ptr == 1) { //not 'if(ptr != 0)', because cannot be 'ref string' or 'ref StringBuilder'
							ptr = 0;
							bool isBSTR = _TokIs(iTokTypename, "BSTR");

							switch(context) {
							case _TypeContext.Member:
								if(isConst || isBSTR) {
									name = "string"; //must be string, not StringBuilder
									if(isBSTR) marshalAs = "BStr";
								} else {
									//string dangerous, because if the callee changes member pointer, .NET tries to free the new string with CoTaskMemFree.
									name = "IntPtr";
									//Print(_DebugGetLine(iTokTypename));
									isBlittable = true;
								}
								break;
							case _TypeContext.Return:
							case _TypeContext.ComReturn:
								name = "IntPtr"; //if string/StringBuilder, .NET tries to free the return value. Use Marshal.PtrToStringUni.
								isBlittable = true;
								break;
							case _TypeContext.DelegateParameter:
								name = "IntPtr"; //because some are "LPSTR or WORD". Rarely used. Use Marshal.PtrToStringUni.
								isBlittable = true;
								break;
							case _TypeContext.Parameter:
							case _TypeContext.ComParameter:
								Debug.Assert(!(isConst && (_Out_ || _Inout_)));
								if(isConst || _In_ || isBSTR) {
									name = "string";
									if(isCOM) {
										//if(!isBSTR) Print(_tok[iTokTypename]);
										if(!isBSTR) marshalAs = "LPWStr";
									} else {
										if(isBSTR) marshalAs = "BStr";
									}
								} else {
									name = _Out_ ? "[Out] StringBuilder" : "StringBuilder";
								}
								break;
							}
						} else if(ptr > 1) {
							ptr--;
							name = "IntPtr";
							isBlittable = true;
							isConst = false;
						}
						break;
					case "int":
						if(_TokIs(iTokTypename, "BOOL")) {
							if(ptr == 0 || (ptr == 1 && isParameter)) {
								name = "bool";
								if(isCOM) marshalAs = "Bool";
								isBlittable = false;
							}
						}
						break;
					case "bool":
						marshalAs = "U1";
						isBlittable = false;
						break;
					case "void":
					case "sbyte":
						if(ptr > 0) {
							name = "IntPtr";
							ptr--;
						}
						break;
					case "double":
						if(_TokIs(iTokTypename, "DATE")) {
							if(ptr == 0 || (ptr == 1 && isParameter)) {
								name = "DateTime";
								isBlittable = false;
							}
						}
						break;
					}
				}
			} else {
				name = t.csTypename;
			}

			if(!isLangType) {
				var ts = t as _Struct;
				if(ts != null) {
					if(ts.isInterface) {
						//OutList(ptr, ts.csTypename);
						if(ptr == 0) _Err(iTokTypename, "unexpected");
						ptr--;
						switch(name) {
						case "IUnknown":
							marshalAs = name;
							name = "Object";
							break;
							//case "IDispatch": //not sure should this be used, because we don't convert SDK interfaces to C# IDispatch interfaces
							//	marshalAs = name;
							//	name = "Object";
							//	break;
						}
					} else if(!ts.isClass) {
						switch(name) {
						case "Wnd": case "LPARAM": isBlittable = true; break;
						case "GUID": name = "Guid"; break;
						case "DECIMAL": name = "decimal"; break;
						case "VARIANT":
							if(context == _TypeContext.ComParameter) name = "object";
							break;
						//case "SAFEARRAY": //in SDK used only with SafeArrayX functions, with several other not-important functions and as [PROP]VARIANT members
						//Print(ptr);
						//break;
						case "ITEMIDLIST":
							if(ptr > 0) { ptr--; name = "IntPtr"; isBlittable = true; }
							//else _Err(iTokTypename, "example"); //0 in SDK
							break;
                        case "POINTL": name = "POINT"; break;
						case "RECTL": name = "RECT"; break;
						}
					}
				} else if(t is _Enum) {
					isBlittable = true;
				}
			}

			if(ptr > 0) {
				__ctn.Clear();
				bool isArray = false;
				if(isParameter) {
					string sal = null;
					if(iSAL > 0) sal = _TokToString(iSAL);

					if(iSAL > 0 && ptr == 1) { //if ptr>1, can be TYPE[]* or TYPE*[]
						if(_In_ && (sal.Contains("_reads_") || sal.Contains("count"))) {
							//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
							isArray = true;
							__ctn.Append("[In] ");
						}
						if(_Inout_ && (sal.Contains("_updates_") || sal.Contains("count"))) {
							//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
							isArray = true;
							__ctn.Append("[In,Out] ");
						}
						if(_Out_ && (sal.Contains("_writes_") || sal.Contains("count"))) {
							//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
							isArray = true;
							__ctn.Append("[Out] ");
						}
					}

					if(_Inout_) isConst = false;

					if(isArray) {
						ptr--;
					} else {
						ptr--;

						if(_Out_) {
							__ctn.Append("out ");
						} else if(_In_ || isConst) {
							if(isBlittable) {
								if(isConst && ptr == 0 && name != "IntPtr") {
									//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
									isArray = true; //usually array, because there is no sense for eg 'const int* param', unless it is a 64-bit value (SDK usually then uses LARGE_INTEGER etc, not __int64). Those with just _In_ usually are not arrays, because for arrays are used _In_reads_ etc.
								} else {
									__ctn.Append("ref ");
									//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
								}
							} else {
								if(isConst) {
									__ctn.Append("[In] ref "); //prevents copying non-blittable types back to the caller when don't need
								} else {
									//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
									//__ctn.Append("[In] ref "); //no, because there are many errors in SDK where inout parameters have _In_
									__ctn.Append("ref ");
								}
							}
						} else {
							__ctn.Append("ref ");
						}
					}

					if(isArray) {
						bool useMarshalAsSubtype = false;
						if(isCOM) {
							//by default marshals as SAFEARRAY
							if(marshalAs == null) marshalAs = "LPArray";
							else useMarshalAsSubtype = true;
						} else if(context == _TypeContext.DelegateParameter) {
							isArray = false;
							ptr++;
							__ctn.Clear();
							//maybe can be array, not tested. Never mind, in SDK only 4 rarely used.
							//OutList(_tok[iTokTypename], name, _DebugGetLine(iTokTypename));
						} else if(marshalAs != null) useMarshalAsSubtype = true;

						if(useMarshalAsSubtype) marshalAs = "LPArray, ArraySubType = UnmanagedType." + marshalAs;
					}
				} //if(isParameter)

				__ctn.Append(name);
				if(ptr > 0) __ctn.Append('*', ptr);
				if(isArray) __ctn.Append("[]");

				name = __ctn.ToString();
			}

			if(marshalAs != null) {
				__ctn.Clear();
				string ret = null; if(context == _TypeContext.Return || context == _TypeContext.ComReturn) ret = "return: ";
				attributes = $"[{ret}MarshalAs(UnmanagedType.{marshalAs})]";
			}
			return name;
		}
		StringBuilder __ctn = new StringBuilder();

		/// <summary>
		/// Converts C-style fixed-size array to C#. Gets MarshalAs attribute, appends "[]" to typeName etc.
		/// Supports "[y][x]" etc.
		/// _i must be at '['. Finally it will be after ']'.
		/// If empty array (like TYPE x[] or TYPE x[0]), attributes receives "//...". If 1-element array, receives "/*...*/".
		/// </summary>
		/// <param name="typeName">This function appends "[]" if need, or convertes "char" to "string".</param>
		/// <param name="memberName">If less than 8 elements, and memberName looks like a "Reserved" etc, splits into multiple members like "Reserved_0, Reserved_1, ...".</param>
		/// <param name="attributes">If !isParameter, receives attribute like "[MarshalAs(UnmanagedType.ByValArray, SizeConst = {x})]". If already is MarshalAs attribute, uses it as ArraySubType.</param>
		/// <param name="isParameter">Just skip [..] or [...][...] and append "[]" to typeName. Used for parameters, because the attribute can be used only with struct fields; C++ arguments for parameters 'TYPE param[n]' are passed like 'TYPE* param', and n is ignored, can be empty.</param>
		void _ConvertCArray(ref string typeName, ref string memberName, ref string attributes, bool isParameter = false)
		{
			uint elemCount = 1;
			for(; _TokIsChar(_i, '['); _i++) { //support [a][b]
				int i0 = _i + 1;
				_SkipEnclosed();
				if(isParameter) continue;
				uint ec = 0;
				if(_i > i0) { //else TYPE x[]
					_ExpressionResult r = _Expression(i0, _i, "[]");
					switch(r.typeS) { case "int": case "uint": break; default: _Err(i0, "cannot calculate"); break; }
					ec = r.valueI;
				}
				elemCount *= ec;
			}

			if(isParameter) {
				typeName += "[]";
				return;
			}

			if(typeName.EndsWith_("[]")) typeName = typeName.Remove(typeName.Length - 2); //'TYPE a[n], b[m]'

			string marshalAs = "ByValArray", comment = null, comment2 = null;
			if(elemCount == 0) { //variable-length array of >= 0 elements
				comment = "//"; //disable the attribute together with member, and let the member be not array
			} else if(elemCount == 1) { //variable-length array of >= 1 elements
				comment = "/*"; comment2 = "*/"; //disable the attribute, and let the member be not array
			} else if(typeName == "char") {
				typeName = "string";
				marshalAs = "ByValTStr";
			} else {
				if(elemCount < 8 && (memberName.IndexOf_("Reserved", true) >= 0 || memberName.IndexOf_("pad", true) >= 0 || memberName.StartsWith_("Spare", true))) {
					//Print(memberName);
					var sb = new StringBuilder();
					for(int i = 0; i < elemCount; i++) {
						if(i > 0) sb.Append(", ");
						sb.Append(memberName);
						sb.Append('_');
						sb.Append(i);
					}
					memberName = sb.ToString();
					comment = "/*"; comment2 = "*/";
				} else {
					//if(elemCount<8) Print(memberName);
					typeName += "[]";
				}
			}

			if(attributes != null) _Err(_i, "TODO"); //0 in SDK. Should extract its type from [MarshalAs(UnmanagedType.(\w+) and insert in the new attributes.
			attributes = $"{comment}[MarshalAs(UnmanagedType.{marshalAs}, SizeConst = {elemCount})]{comment2}";
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
