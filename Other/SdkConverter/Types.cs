using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;

namespace SdkConverter
{
	unsafe partial class Converter
	{
		/// <summary>
		/// Converts simple typedef or function typedef declaration.
		/// _i must be at 'typedef'. Finally it will be at semicolon.
		/// </summary>
		void _DeclareTypedefTypeOrFunc()
		{
			var d = new _FINDTYPEDATA();
			_i = _FindTypename(false, ++_i, ref d);
			int iTypeName = _i++;

			//is function?
			bool isFunction = false;
			int i = _i, ptr = 0;
			for(;;) {
				if(_TokIsChar(i, '*', '&')) { i++; ptr++; } else {
					isFunction = _DetectIsFuncType(i);
					break;
				}
			}

			if(isFunction) {
				_i = i;
				_DeclareTypedefFunc(d.outSym, ptr, d.outIsConst, iTypeName);
			} else {
				_DeclareTypedef(d.outSym, d.outIsConst);
			}
		}

		/// <summary>
		/// Converts function typedef to C# delegate.
		/// _i must be at the first '(' (after typename and * etc). Finally it will be at semicolon.
		/// Info: script replaced 'typedef TYPE [callConv] FUNCTYPE(' to 'typedef TYPE ([callConv]*FUNCTYPE)('.
		/// </summary>
		/// <param name="t">Return type.</param>
		/// <param name="ptr">Pointer level.</param>
		/// <param name="isConst">Is const.</param>
		/// <param name="iTokTypename">Type name token index. Not used if nestedStructName not null.</param>
		/// <param name="nestedStructName">Use when the type is now defined nested struct/enum.</param>
		/// <param name="inl">Use for inline function type definition.</param>
		void _DeclareTypedefFunc(_Symbol t, int ptr, bool isConst, int iTokTypename, string nestedStructName = null, _INLINEFUNCTYPEDATA inl = null)
		{
			//return type
			string type = _ConvertTypeName(t, ref ptr, false, isConst, iTokTypename, nestedStructName);

			Debug.Assert(_TokIsChar(_i, '('));
			_i++;

			//calling convention
			int iCallConv = 0;
			if(_TokIsIdent(_i)) iCallConv = _i++;
			if(!_TokIsChar(_i, '*')) _Err(_i, "unexpected"); //todo: maybe also can be & (use _TokIsPtr)
			_i++;
			string callConv = iCallConv != 0 ? _ConvertCallConv(iCallConv) : "Cdecl";

			//name
			int iName = _i;
			string name = null;
			if(inl == null) {
				if(_SymbolExists(_i, false)) _Err(_i, "name already exists");
				name = _TokToString(_i);
			} else {
				if(_TokIsIdent(_i)) inl.paramName = _TokToString(_i);
				else { _i--; inl.paramName = "param" + inl.paramIndex; }

				inl.typeName = name = (inl.parentName + "_" + inl.paramName);
			}
			if(!_TokIsChar(++_i, ')')) _Err(_i, "unexpected");
			_i++;

			//start formatting
			StringBuilder sb = inl != null ? inl.sb : _sbType;
			if(callConv != null) sb.AppendFormat("[UnmanagedFunctionPointer(CallingConvention.{0})]\r\n", callConv);
			bool isUnsafe = ptr != 0; int unsafeInsertPos = sb.Length + 6;
			sb.AppendFormat("public delegate {0} {1}", type, name);

			//parameters
			var p = inl != null ? inl.parms : _params;
			_GetParameters(p);
			int iTokFinally = _i + 1;
			_ConvertParameters(p, ref sb, ref isUnsafe, name);

			//finish formatting
			if(isUnsafe) sb.Insert(unsafeInsertPos, " unsafe");

			_i = iTokFinally;

			var x = new _TypedefFunc();
			_AddSymbol(name, x, iName);

			if(inl != null) {
				inl.t = x;
				inl.isUnsafe = isUnsafe;
			}
		}

		class _INLINEFUNCTYPEDATA
		{
			public readonly List<_PARAMETER> parms; //in
			public readonly StringBuilder sb; //ref
			public readonly string parentName; //in
			public int paramIndex; //in
			public string typeName; //out
			public string paramName; //out
			public _TypedefFunc t; //out
			public bool isUnsafe; //out

			public _INLINEFUNCTYPEDATA(string parentName)
			{
				parms = new List<_PARAMETER>();
				sb = new StringBuilder();
				this.parentName = parentName;
			}
		}

		/// <summary>
		/// Processes non-function typedef declaration.
		/// Scans the list of aliases and declares all.
		/// Starts from token index _i, which must be after base name (at the first alias or * in the list). Finally it will be at semicolon.
		/// </summary>
		/// <param name="aliasOf">The type for which are declared aliases.</param>
		void _DeclareTypedef(_Symbol aliasOf, bool isConst)
		{
			int ptr = 0; //0 - start or after comma, >0 pointer level, <0 after identifier
			int ptrBase = 0;
			int iBaseTypeName = _i - 1;

			//don't declare typedef to typedef
			if(aliasOf is _Typedef) {
				var t = aliasOf as _Typedef;
				aliasOf = t.aliasOf;
				ptrBase = t.ptr;
			}

			for(; ; _i++) {
				if(_TokIsIdent(_i)) {
					if(ptr < 0) goto ge;
					if(_TokIsChar(_i + 1, '[')) { //typedef X Y[n];
						var x = new _Struct(false);
						_AddSymbol(_i, x);
						string name = _TokToString(_i++);
						string type = _ConvertTypeName(aliasOf, ref ptr, false, false, iBaseTypeName);
						string attr;
						_i = _CArrayToMarshalAsAttribute(_i, out attr) - 1;
						x.cs = $"public struct {name} {{\r\n\t{attr} {type}[] a;\r\n}}\r\n";
						//info:
						//SDK has 6 such typedefs, 2 of them are [1] ie used as variable-length array.
						//We convert to struct, because would be difficult to have this as typedef.
					} else {
						_AddSymbol(_i, new _Typedef(aliasOf, ptr + ptrBase, isConst));
					}
					ptr = -1;
				} else {
					switch(*T(_i)) {
					case '*':
					case '&':
						if(ptr < 0) goto ge;
						ptr++;
						break;
					case ',':
						if(ptr >= 0) goto ge;
						ptr = 0;
						break;
					case ';':
						if(ptr >= 0) goto ge;
						return;
					default: goto ge;
					}
				}
			}
			ge:
			_Err(_i, "unexpected");
		}

		/// <summary>
		/// Converts enum declaration.
		/// _i must be at 'enum'. Finally it will be at semicolon.
		/// </summary>
		void _DeclareEnum(ref _FINDTYPEDATA d)
		{
			//scoped?
			if(_TokIs(++_i, "class") || _TokIs(_i, "struct")) _i++;

			//name
			int iName = 0;
			if(_TokIsIdent(_i)) {
				iName = _i++;

				//is forward declaration? Rare but allowed.
				if(_TokIsChar(_i, ';')) {
					_AddSymbol(iName, d.outSym = new _Enum(true));
					//info: don't need name in this case
					return;
				}
			}

			_ns[_nsCurrent + 1].Clear();
			StringBuilder sb = _ns[_nsCurrent + 1].sb;

			//base type
			string sBaseType = null;
			if(_TokIsChar(_i, ':')) {
				_Symbol baseType = _FindType(++_i, true);
				int ptr = _Unalias(_i, ref baseType);
				if(!(baseType is _CppType) || ptr != 0) _Err(_i, "unexpected");
				sBaseType = baseType.cs;
				_i++;
			}

			if(!_TokIsChar(_i, '{')) _Err(_i, "unexpected");
			int iBodyStart = _i++;

			if(iName == 0) {
				//borrow enum name from first member. Benefits: easier for users to find the enum; will not be a name conflict; easy.
				if(!_TokIsIdent(_i)) _Err(_i, "unexpected");
				iName = _i;
			}

			string name = _TokToString(iName);

			//start formatting
			sb.Append("public enum ");
			sb.Append(name);
			if(sBaseType != null) { sb.Append(" :"); sb.Append(sBaseType); }
			sb.Append(' ');

			_sbType = _ns[++_nsCurrent].sb;

			//body
			for(;;) {
				char* s = T(_i++);
				if(*s == '}') break;
				if(*s == '\0') _Err(iBodyStart, "no }");
				if(sBaseType == null && *s == '0' && (s[1] == 'x' || s[1] == 'X')) sBaseType = "uint";
			}
			int iBodyEnd = _i;

			_sbType = _ns[--_nsCurrent].sb;

			if(sBaseType == "uint") sb.Insert(0, "[Flags]\r\n");
			sb.AppendLine(new string(T(iBodyStart), 0, (int)(T(iBodyEnd - 1) + 1 - T(iBodyStart))));

			_Enum x;
			if(_TryFindSymbolAs(iName, out x, false) && x.forwardDecl == true) x.forwardDecl = false;
			else _AddSymbol(iName, x = new _Enum(false));

			x.cs = sb.ToString();
			_sbType.Append(x.cs);

			d.outSym = x;
			d.outTypename = name;
		}

		/// <summary>
		/// Converts struct/union/class/__interface/enum/typedef definition or forward declaration.
		/// _i must be at 'struct' etc. Finally it will be at ; (if global) or after } (if nested).
		/// </summary>
		void _DeclareType(bool isMember, ref _FINDTYPEDATA d)
		{
			//struct, union, class, or __interface?
			bool isUnion = false, isInterface = false;
			string access = "public";
			switch(*T(_i)) {
			case 't': _DeclareTypedefTypeOrFunc(); return;
			case 'e': _DeclareEnum(ref d); return;
			case 'u': isUnion = true; break;
			case 'c': access = "private"; break;
			case '_': isInterface = true; break;
			}
			_i++;

			//uuid (was '__declspec(uuid', replaced in script)
			string uuid = null;
			if(_TokIs(_i, "uuid")) {
				if(!_TokIsChar(++_i, '(') || !_TokIsChar(++_i, '\"') || !_TokIsChar(_i + 1, ')')) _Err(_i, "unexpected");
				uuid = _TokToString(_i);
				_i += 2;
			}

			//name
			_Struct x = null;
			int iName = 0;
			string name;
			if(_TokIsIdent(_i)) {
				iName = _i++;

				//forward-declare this because may contain a member of this pointer type
				if(!_TryFindSymbolAs(iName, out x, false) || !x.forwardDecl) _AddSymbol(iName, x = new _Struct(true));

				//is forward declaration?
				if(_TokIsChar(_i, ';')) {
					d.outSym = x;
					//info: don't need name in this case
					return;
				}

				name = _TokToString(iName);
			} else {
				//later will get name from variable and replace placeholder "\x1"
				name = "\x1";
			}

			_ns[_nsCurrent + 1].Clear();
			StringBuilder sb = _ns[_nsCurrent + 1].sb;

			//start formatting
			if(_pack != 8) sb.AppendFormat("[StructLayout(LayoutKind.{0}, Pack={1})]\r\n", isUnion ? "Explicit" : "Sequential", _pack);
			else if(isUnion) sb.Append("[StructLayout(LayoutKind.Explicit)]\r\n");
			sb.Append("public struct ");
			int nameOffset = sb.Length;
			sb.Append(name);
			sb.Append(" {\r\n");
			int membersOffset = sb.Length - 2;

			//base type
			if(_TokIsChar(_i, ':')) {
				_i++;
				if(_TokIsIdent(_i + 1) && _FindKeyword(_i).kwType == _KeywordT.PubPrivProt) _i++;

				for(; ; _i++) { //support multiple inheritance
					_Symbol baseType = _FindType(_i, true);
					int ptr = _Unalias(_i, ref baseType);
					if(!(baseType is _Struct) || baseType.forwardDecl || ptr != 0) _Err(_i, "unexpected");
					sb.Append(baseType.cs);
					if(!_TokIsChar(++_i, ',')) break;
				}
			}

			//members
			if(!_TokIsChar(_i, '{')) _Err(_i, "unexpected");
			int iBodyStart = _i++;
			_INLINEFUNCTYPEDATA f = null;
			string attrFieldOffset = isUnion ? "[FieldOffset(0)]\r\n" : null;
			bool hasBitfields = false;

			if(_nsCurrent == 0) _anonymousStructSuffixCounter = 0;
			_sbType = _ns[++_nsCurrent].sb;

			for(int iMember = 1, iBitfields = 1; !_TokIsChar(_i, '}'); _i++, iMember++) {

				//public/private/protected?
				if(_TokIsChar(_i + 1, ':') && 0 != _TokIs(_i, "public", "private", "protected")) {
					access = _TokToString(_i);
					_i++;
					continue;
				}

				//bitfield?
				if(__IsBitfield(_i)) {
					_Bitfields(ref iBitfields, attrFieldOffset);
					hasBitfields = true;
					continue;
				}

				//type
				_PARAMDATA t;
				_i = _ParseParamOrMember(true, _i, out t, ref f, "TYPEOF", iMember);

				if(t.isNestedTypeDefinitionWithoutVariables) {
					//if anonymous type definition without a variable, its members are part of parent in C++, therefore in C# need to add a variable of that type
					if(t.isAnonymousTypeDefinitionWithoutVariables) {
						sb.AppendFormat("{3}{2} {0} {1};\r\n", t.typeName, "_" + iMember, access, attrFieldOffset);
					}
					continue;
				}

				//member variable
				for(;;) { //support TYPE a, b;
					switch(*T(_i)) {
					case ';': case ',': break;
					case '\0': _Err(iBodyStart, "no }"); break;
					default: _Err(_i, "unexpected"); break;
					}

					if(t.attributes != null) sb.AppendFormat("{0}\r\n", t.attributes);
					sb.AppendFormat("{3}{2} {0} {1};\r\n", t.typeName, t.name, access, attrFieldOffset);
					if(!_TokIsChar(_i, ',')) break;
					_i++;

					//ignore *. Assume never will be 'TYPE *a, b;' or 'TYPE a, *b;.
					while(_TokIsChar(_i, '*', '&')) _i++;
					//int ptr = 0;
					//while(_TokIsChar(_i, '*', '&')) { _i++; ptr++; }
					//Out(ptr);

					if(!_TokIsIdent(_i)) _Err(_i, "unexpected");
					t.name = _TokToString(_i++);

					if(_TokIsChar(_i, '[')) {
						_i = _CArrayToMarshalAsAttribute(_i, out t.attributes);
						if(!t.typeName.EndsWith_("[]")) t.typeName += "[]";
					} else if(t.attributes != null) {
						t.attributes = null;
						if(t.typeName.EndsWith_("[]")) t.typeName = t.typeName.Remove(t.typeName.Length - 2);
					}
				}

				//if(t.isUnsafe) isUnsafe = true; //TODO
			}

			_sbType = _ns[--_nsCurrent].sb;

			_i++; //skip }
			if(!isMember) {
				//skip variables after }
				while(!_TokIsChar(_i, ';', '\0')) _i++;
			}

			if(iName == 0) {
				//if struct member, use variable name from 'struct{...}var;', else 
				if(!isMember) return; //skip this struct/var definition. Don't need to fill d in this case.

				bool isVar = _TokIsIdent(_i);
				name = isVar ? ("TYPEOF_" + _TokToString(_i)) : ("TYPE_" + (++_anonymousStructSuffixCounter).ToString());
				sb.Replace("\x1", name, nameOffset, 1);
				_AddSymbol(name, x = new _Struct(false), _i);

				d.outIsAnonymousTypeDefinition = true;
			} else {
				x.forwardDecl = false;
			}

			if(f != null) sb.Append(f.sb.ToString());
			sb.Replace("\n", "\n\t", membersOffset, sb.Length - 2 - membersOffset);
			sb.Append("}\r\n");
			if(hasBitfields) sb.Insert(0, "[DebuggerStepThrough]\r\n");
			//TODO: unsafe

			x.cs = sb.ToString();
			_sbType.Append(x.cs);

			d.outSym = x;
			d.outTypename = name;
		}

		void _DeclareInterface()
		{
			_Err(_i, "not impl"); //TODO

		}

		void _Bitfields(ref int iBitfields, string attrFieldOffset)
		{
			int nBitsType = 0, nBitsTypePrev = 0, fieldOffset = 0;
			string csTypeName = null, bitHolderVar = null;
			bool isUnsigned;

			for(;;) {
				//type
				nBitsType = __BitfieldGetType(_i, out csTypeName, out isUnsigned);
				if(nBitsType != nBitsTypePrev) { nBitsTypePrev = nBitsType; bitHolderVar = null; }
				_i++;
				g1:
				//name
				string varName = _TokIsChar(_i, ':') ? null : _TokToString(_i++);
				_i++;
				//field size
				int fieldSize = Api.strtoi(T(_i));
				if(fieldSize < 1 || fieldSize > nBitsType) _Err(_i, "unexpected");
				_i++;
				//add private member variable that holds the bitfields
				if(bitHolderVar == null || fieldOffset + fieldSize > nBitsType) {
					fieldOffset = 0;
					bitHolderVar = "__bf_" + iBitfields++;
					_sbType.AppendFormat("{2}private {0} {1};\r\n", csTypeName, bitHolderVar, attrFieldOffset);
				}
				//add property that get/set the bitfield
				if(varName != null) {
					ulong mask = (1UL << fieldSize) - 1;
					ulong maskWithOffset = mask << fieldOffset;

					//work around C# problems with signed/unsigned conversion
					string op = "~";
					if(nBitsType > 32) {
						if(maskWithOffset > long.MaxValue) { maskWithOffset = ~maskWithOffset; op = null; }
					} else {
						if(maskWithOffset > int.MaxValue) { maskWithOffset = ~maskWithOffset & 0xffffffff; op = null; }
					}

					_sbType.AppendFormat(
						"public {0} {1} {{ get {{ return {8}({2} >> {3} & 0x{4:X}{7}); }} " +
						"set {{ {2} = {8}(({2} & {6}0x{5:X}{7}) | ((value & 0x{4:X}{7}) << {3})); }} }}\r\n",
						csTypeName, varName, bitHolderVar, fieldOffset, mask, maskWithOffset,
						op, isUnsigned ? "U" : "", nBitsType < 32 ? "(" + csTypeName + ")" : "");
				}

				fieldOffset += fieldSize;

				//next bitfield?
				if(_TokIsChar(_i++, ',')) {
					if(!(_TokIsChar(_i + 1, ':') && _TokIsIdent(_i) && _TokIsChar(_i + 3, ';', ','))) _Err(_i, "unexpected");
					goto g1;
				}
				if(!__IsBitfield(_i)) break;
			}

			_i--;
		}

		bool __IsBitfield(int i)
		{
			if(_TokIsChar(i + 2, ':')) {
				if(!_TokIsIdent(i++)) return false;
			} else if(!_TokIsChar(i + 1, ':')) return false; //nameless?

			return _TokIsIdent(i) && _IsCharDigit(*T(i + 2)) && _TokIsChar(i + 3, ';', ',');
		}

		/// <summary>
		/// Returns bitfield size in bits. Gets C# type name.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		int __BitfieldGetType(int i, out string csTypeName, out bool isUnsigned)
		{
			csTypeName = null;
			isUnsigned = false;
			_Symbol t = _FindType(i, true);
			if(0 == _Unalias(i, ref t)) {
				var ct = t as _CppType;
				if(ct != null && ct.sizeBytesCpp != 0) {
					csTypeName = ct.cs;
					isUnsigned = ct.isUnsigned;
					return ct.sizeBytesCpp * 8;
				} else if(_TokIs(i, "IntLong")) {
					csTypeName = "long";
					return 64;
					//SDK has 1 such struct (union PSAPI_WORKING_SET_BLOCK), and there the bitfield is at the end, so in most cases it is safe to use C# long instead. Using IntLong is difficult because of its variable size.
				}
			}
			_Err(i, "unexpected");
			return 0;
		}
	}
}
