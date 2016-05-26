#define TEST_SMALL

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
	class Program
	{
		static void Main(string[] args)
		{
			Output.Clear();
			var x = new Converter();
			x.Convert(
#if TEST_SMALL
				//@"Q:\app\Catkeys\Other\SdkConverter\Data\Header.h",
				@"Q:\app\Catkeys\Other\SdkPreprocess\Cpp.cpp",
#else
				@"Q:\app\Catkeys\Api\Api-preprocessed.cpp",
#endif
				@"Q:\app\Catkeys\Api\Api.cs"
);


			//ConsoleDriver.Run(new SdkLibrary());
		}
	}

	unsafe partial class Converter
	{
		char[] _src; //C/C++ source code
		char* _s0; //_src start

		StringBuilder _sbConst = new StringBuilder();
		StringBuilder _sbEnum = new StringBuilder();
		StringBuilder _sbStruct = new StringBuilder();
		StringBuilder _sbDelegate = new StringBuilder();
		StringBuilder _sbFunc = new StringBuilder();
		StringBuilder _sbInterface = new StringBuilder();
		StringBuilder _sbCoclass = new StringBuilder();
		StringBuilder _sbComment = new StringBuilder();

		List<_Token> _tok = new List<_Token>();
		int _i; //current token

		Dictionary<_Token, _Symbol> _sym = new Dictionary<_Token, _Symbol>(); //all symbols that we need when parsing - C++ keywords/types, enum, struct, typedef, etc

		List<string> _comment = new List<string>(); //added when cannot convert: typedef STRUCT, #define MACRO(y), etc
		Dictionary<string, string> _defineConst = new Dictionary<string, string>(); //#define CONSTANT
		Dictionary<string, string> _defineOther = new Dictionary<string, string>(); //#define MACRO(...
																					//List<string> _cppConst = new List<string>(); //const CONSTANT
		Dictionary<string, string> _func = new Dictionary<string, string>(); //function declaration

		Stack<int> _packStack = new Stack<int>(); int _pack = 8; //#pragma pack

		public void Convert(string hFile, string csFile)
		{
			try {
				if(_src != null) throw new Exception("cannot call Convert multiple times. Create new Converter instance.");

				using(var reader = new StreamReader(hFile)) {
					long n = reader.BaseStream.Length + 4; if(n > int.MaxValue / 4) throw new Exception("file too big");
					reader.Read(_src = new char[n], 0, (int)n);
				}

				_InitTables();
				_InitSymbols();

				fixed (char* p = _src)
				{
					_s0 = p;
					_Convert(p);
				}

				string s = @"
using System;
using System.Runtime.InteropServices;
using System.Text;

//add this to projects that will use these API
[module: DefaultCharSet(CharSet.Unicode)]

public static class API
{
";

				using(var writer = new StreamWriter(csFile)) {
					writer.Write(s);
					writer.Write("\r\n// CONST\r\n\r\n");
					writer.Write(_sbConst.ToString());
					writer.Write("\r\n// ENUM\r\n\r\n");
					writer.Write(_sbEnum.ToString());
					writer.Write("\r\n// STRUCT\r\n\r\n");
					writer.Write(_sbStruct.ToString());
					writer.Write("\r\n// DELEGATE\r\n\r\n");
					writer.Write(_sbDelegate.ToString());
					writer.Write("\r\n// FUNCTION\r\n\r\n");
					writer.Write(_sbFunc.ToString());
					writer.Write("\r\n// INTERFACE\r\n\r\n");
					writer.Write(_sbInterface.ToString());
					writer.Write("\r\n// COCLASS\r\n\r\n");
					writer.Write(_sbCoclass.ToString());
					writer.Write("\r\n// CANNOT CONVERT\r\n\r\n");
					writer.Write(_sbComment.ToString());
					writer.Write("\r\n}\r\n");
				}
			}
			//#if !TEST_SMALL
			catch(ConverterException e) {
				Wnd.FindCN("QM_Editor").SendS(Api.WM_SETTEXT, 1, $"M \"api_converter_error\" A(||) {e.Message}||{hFile}||{e.Offset}");
			} catch(Exception e) {
				Out(e);
				Wnd.FindCN("QM_Editor").SendS(Api.WM_SETTEXT, 1, $"M \"api_converter_error\" A(||) {" "}||{hFile}||{_Pos(_i)}");
			}
			//#endif
			finally {
				Marshal.FreeHGlobal((IntPtr)_keywordMemory);
			}
		}

		char* _keywordMemory;
		char* _km; //curent keyword

		void _AddKeyword(string name, _Symbol sym)
		{
			char* n = _km;
			for(int i = 0; i < name.Length; i++) *_km++ = name[i];
			_sym.Add(new _Token(n, (int)(_km - n)), sym);
			*_km++ = '\0';
		}

		void _AddKeywords(_Symbol sym, params string[] names)
		{
			foreach(string s in names) {
				_AddKeyword(s, sym);
			}
		}

		void _InitSymbols()
		{
			_km = _keywordMemory = (char*)Marshal.AllocHGlobal(4000);

			//Add C++ keywords and types, except those that can only be in function body or other places where we skip.
			//The commented keywords/types are currently not supported and not expected to be in C/C++ headers that we'll parse.

			_Keyword k;
			//_Enum e;
			//_Struct s;
			//_Typedef td;
			//_TypedefFunc tf;

			//_AddKeyword("__alignof", k = new _Keyword(cannotStartStatement:true));
			//_AddKeywords(k = new _Keyword(cannotStartStatement:true), "__based", "_based");
			_AddKeywords(k = new _Keyword(), "__declspec", "_declspec");
			//_AddKeyword("__event", k = new _Keyword());
			//_AddKeyword("__identifier", k = new _Keyword());
			//_AddKeywords(k = new _Keyword(), "__if_exists", "_if_exists", "__if_not_exists", "_if_not_exists");
			//_AddKeywords(k = new _Keyword(), "__multiple_inheritance", "_multiple_inheritance", "__single_inheritance", "_single_inheritance", "__virtual_inheritance", "_virtual_inheritance"); k.cannotStartStatement = true;
			_AddKeyword("__uuidof", k = new _Keyword(cannotStartStatement: true));
			_AddKeyword("const", k = new _Keyword());
			_AddKeyword("extern", k = new _Keyword());
			_AddKeyword("false", k = new _Keyword(cannotStartStatement: true));
			_AddKeyword("namespace", k = new _Keyword());
			_AddKeyword("nullptr", k = new _Keyword(cannotStartStatement: true));
			_AddKeyword("operator", k = new _Keyword());
			_AddKeyword("property", k = new _Keyword());
			//_AddKeyword("signed", k = new _Keyword()); //our script replaced it
			_AddKeyword("sizeof", k = new _Keyword(cannotStartStatement: true));
			_AddKeyword("static", k = new _Keyword());
			_AddKeyword("static_assert", k = new _Keyword());
			_AddKeyword("template", k = new _Keyword());
			_AddKeyword("throw", k = new _Keyword(cannotStartStatement: true)); //eg void f() throw(int);
			_AddKeyword("true", k = new _Keyword(cannotStartStatement: true));
			//_AddKeyword("unsigned", k = new _Keyword()); //our script replaced it
			_AddKeyword("using", k = new _Keyword());
			_AddKeyword("virtual", k = new _Keyword());

			k = new _Keyword(_KeywordT.TypeDecl);
			_AddKeyword("__interface", k);
			_AddKeyword("class", k);
			_AddKeyword("enum", k);
			_AddKeyword("struct", k);
			_AddKeyword("typedef", k);
			_AddKeyword("union", k);

			k = new _Keyword(_KeywordT.CallConv, cannotStartStatement: true);
			_AddKeyword("__cdecl", k); _AddKeyword("_cdecl", k);
			_AddKeyword("__fastcall", k); _AddKeyword("_fastcall", k);
			_AddKeyword("__stdcall", k); _AddKeyword("_stdcall", k);
			_AddKeyword("__thiscall", k);

			k = new _Keyword(_KeywordT.Inline);
			_AddKeyword("__forceinline", k);
			_AddKeyword("__inline", k);
			_AddKeyword("_inline", k);
			_AddKeyword("inline", k);

			k = new _Keyword(_KeywordT.PubPrivProt);
			_AddKeyword("private", k);
			_AddKeyword("protected", k);
			_AddKeyword("public", k);

			k = new _Keyword(_KeywordT.Ignore);
			_AddKeyword("__unaligned", k);
			_AddKeyword("__w64", k); _AddKeyword("_w64", k);
			_AddKeyword("volatile", k);
			_AddKeyword("explicit", k);
			_AddKeyword("friend", k);
			_AddKeyword("mutable", k);

			_CppType t;
			_AddKeyword("__int8", t = new _CppType("sbyte"));
			_AddKeyword("__int16", t = new _CppType("short"));
			_AddKeyword("__int32", t = new _CppType("int"));
			_AddKeyword("__int64", t = new _CppType("long"));
			_AddKeyword("__wchar_t", t = new _CppType("char"));
			_AddKeyword("bool", t = new _CppType("bool"));
			_AddKeyword("char", t = new _CppType("sbyte"));
			_AddKeyword("double", t = new _CppType("double"));
			_AddKeyword("float", t = new _CppType("float"));
			_AddKeyword("int", t = new _CppType("int"));
			_AddKeyword("long", t = new _CppType("int"));
			_AddKeyword("short", t = new _CppType("short"));
			//our preprocessor script replaced 'unsigned type' to 'u$type'
			//also the script replaced 'signed type' to 'type, 'long long' to '__int64', 'long double' to 'double'
			_AddKeyword("u$__int8", t = new _CppType("byte"));
			_AddKeyword("u$__int16", t = new _CppType("ushort"));
			_AddKeyword("u$__int32", t = new _CppType("uint"));
			_AddKeyword("u$__int64", t = new _CppType("ulong"));
			_AddKeyword("u$char", t = new _CppType("byte"));
			_AddKeyword("u$int", t = new _CppType("uint"));
			_AddKeyword("u$long", t = new _CppType("uint"));
			_AddKeyword("u$short", t = new _CppType("ushort"));
			//_AddKeyword("", t = new _CppType(""));
			//_AddKeywords(k = new _Keyword(), "__m128", "__m128d", "__m128i"); //our script defined these as struct, because C# does not have a matching type
			//_AddKeywords(k = new _Keyword(), "__m64"); //not used in SDK
			_AddKeyword("void", t = new _CppType("void"));
			_AddKeyword("auto", t = new _CppType("var")); //can be global variable
		}

		void _Convert(char* s)
		{
			//Test(); return;

			//tokenize
			_tok.Capacity = _src.Length / 8;
			_tok.Add(new _Token());

			bool isNewLine = true;
			int len;
			char c;

			for(; (c = *s) != 0; s++) {
				switch(c) {
				case '\r':
				case '\n':
					isNewLine = true;
					break;
				case ' ':
				case '\t':
				case '\v':
				case '\f':
					break;
				case '@': //was #define, #undef or #pragma pack
					if(!isNewLine || (s[1] != 'd' && s[1] != 'u' && s[1] != '(')) _Err(s, "unexpected");
					_tok.Add(new _Token(s, 1));
					break;
				default:
					if(_IsCharIdentStart(c)) {
						len = _LenIdent(s);

						//is prefix"string" or prefix'char'?
						bool isPrefix = false;
						switch(s[len]) {
						case '\"':
							if(len == 1) isPrefix = (*s == 'L' || *s == 'u' || *s == 'U');
							else if(len == 2) isPrefix = (s[0] == 'u' && s[1] == '8');
							//info: raw strings replaced to escaped strings when preprocessing
							break;
						case '\'':
							if(len == 1) isPrefix = (*s == 'L' || *s == 'u' || *s == 'U');
							break;
						}
						if(isPrefix) { s += len - 1; continue; } //remove prefix

					} else if(_IsCharDigit(c)) len = _LenNumber(s);
					else if(c == '\"') len = _SkipString(s);
					else if(c == '\'') len = _SkipApos(s);
					else len = 1;
					_tok.Add(new _Token(s, len));
					s += len - 1;
					break;
				}
			}
			int nTok = _tok.Count;
			for(int i = 0; i < 10; i++) _tok.Add(new _Token(s, 1)); //to safely query next token
			_tok[0] = new _Token(s, 1); //to safely query previous token. Also used as an empty token. The empty list item added before tokenizing.

			for(_i = 1; _i < nTok; _i++) {
				//Out(_tok[_i].ToString());
				s = T(_i);
				if(*s == '@') { //was #define, #undef or #pragma pack
					if(s[1] == '(') _PragmaPack();
					else _DefineUndef();
				} else if(_IsCharIdentStart(*s)) {
					_Statement();
				} else if(*s != ';') {
#if TEST_SMALL
					if(*s == '/' && s[1] == '/') break;
#endif
					//Out(_i);
					_Err(_i, $"unexpected: {_tok[_i].ToString()}");
				}
			}

			//Out("#define CONST:");
			//Out(_defineConst);
			//Out("#define other:");
			//Out(_defineOther);
			//OutList(_defineConst.Count, _defineOther.Count);
			//27890, 1061
			//27659, 998
			//27605, 983

			//foreach(var v in _defineConst) {
			//	Out($"{v.Key}={v.Value}");
			//}
			foreach(var v in _defineOther) {
				//Out($"{v.Key}={v.Value}");
				_sbComment.AppendFormat("/// #define {0}{1}\r\nconst {0} = \"C macro\";\r\n\r\n", v.Key, v.Value);
			}
			//TODO: finally remove '#define FuncX FuncY', '#define X UnknownIdentifier' etc.


			Out(_sbEnum);
			Out(_sbDelegate);
			Out(_sbStruct);

			//TODO: typedef can be added as 'using X = Y;' or 'public unsafe struct X { Y* p; }'
		}

		void _DefineUndef()
		{
			char* s = T(++_i);
			char c = *s; //was like @d$$$_REALNAME, now s is without @
			s += 5; int lenName = _tok[_i].len - 5; //skip prefix 'd$$$_' that was added to avoid unexpanding names

			//is function-style?
			char* sNext = T(_i + 1);
			bool isFunc = c == 'd' && *sNext == '(' && sNext == s + lenName;

			//find value
			int iValue = _i + 1;
			if(isFunc) iValue = _SkipEnclosed(iValue) + 1; //name(parameters)[ value]

			//find next line
			int iNext = iValue;
			for(; ; iNext++) {
				char k = *T(iNext);
				if(k == '@' || k == '\x0') break;
			}

			string name = new string(s, 0, lenName);
			if(c == 'u') { //#undef
				if(!_defineConst.Remove(name)) _defineOther.Remove(name);
				//Out($"#undef {name}");
			} else if(iValue < iNext) { //preprocessor removes some #define values, it's ok
				s = T(_i + 1); //info: for func-style get parameters as part of value
				int i = iNext - 1;
				string value = new string(s, 0, (int)(T(i) - s) + _tok[i].len);

				if(isFunc) {
					_defineOther[name] = value;
					//Out($"#define {name}{value}");
				} else {
					_defineConst[name] = value;
					//Out($"#define {name} {value}");
				}
			}

			_i = iNext - 1;
		}

		void _PragmaPack()
		{
			const string sErr1 = "expected (push), (pop), (n), (push, n), (pop, n) or ()";
			char* s0 = T(++_i);
			int nArg = _GetParameters(_params), pack = 0, pushPop = 0;

			if(nArg == 0) {
				pack = 8;
			} else if(nArg > 2) {
				_Err(s0, sErr1);
			} else {
				if(_params[0].nTok != 1 || (nArg > 1 && _params[1].nTok != 1)) _Err(s0, sErr1);
				int i = _params[0].iTok;
				char* s = T(i);

				if(_TokIs(i, "push")) pushPop = 1;
				else if(_TokIs(i, "pop")) pushPop = -1;
				else if(nArg == 2) _Err(s0, sErr1);
				else pack = -1;

				if(nArg == 2) {
					s = T(_params[1].iTok);
					pack = -1;
				}

				if(pack < 0) { //a pack value must be specified
					pack = Api.strtoi(s);
					if(pack != 1 && pack != 2 && pack != 4 && pack != 8 && pack != 16)
						_Err(s, "expected 1, 2, 4, 8 or 16");
				}
			}

			if(pushPop > 0) _packStack.Push(_pack); else if(pushPop < 0 && _packStack.Count > 0) _pack = _packStack.Pop();
			if(pack != 0) _pack = pack;
			//Out($"pushPop={pushPop}, pack={pack},    _pack={_pack}, _packStack.Count={_packStack.Count}");
		}

		void _Statement()
		{
			g0:
			_Symbol x = _FindSymbol(_i);
			if(x.symType == _SymT.Keyword) {
				var k = x as _Keyword;
				if(k.cannotStartStatement) _Err(_i, "unexpected");

				if(k.kwType == _KeywordT.Ignore) {
					if(!_TokIsIdent(++_i)) _Err(_i, "unexpected");
					goto g0;
				}

				if(k.kwType == _KeywordT.TypeDecl) {
					bool isTypedef = *T(_i) == 't', isConst = false;
					if(isTypedef) {
						gk1:
						x = _FindSymbol(++_i);
						if(x.symType != _SymT.Keyword) {
							_i++;
							_DeclareTypedefTypeOrFunc(x, isConst);
							goto gSem;
						}
						k = x as _Keyword;
						if(k.kwType != _KeywordT.TypeDecl) {
							if(_TokIs(_i, "const")) { isConst = true; goto gk1; }
							_Err(_i, "unexpected");
						}
					}

					//TODO: can be like 'typedef [const] struct tagX X;'

					switch(*T(_i++)) {
					case 'e': _DeclareEnum(isTypedef); break;
					case 's': _DeclareStruct(isTypedef); break;
					case 'u': _DeclareUnion(isTypedef); break;
					case 'c': _DeclareClass(isTypedef); break;
					case '_': _DeclareInterface(isTypedef); break;
					default: Debug.Assert(false); break;
					}
					gSem:
					if(!_TokIsChar(_i, ';')) _Err(_i, "unexpected");
				} else {
					_Err(_i, "unexpected"); //TODO

				}

			} else { //a type
					 //		 //now we expect one of:
					 //		 //	function declaration, like TYPE __stdcall Func(TYPE a, TYPE b);
					 //		 //	function definition, like TYPE __stdcall Func(TYPE a, TYPE b){body}
					 //		 //	global variable, which can be:
					 //		 //		previously-defined type, like TYPE var;
					 //		 //		now-defined function type, like TYPE (__stdcall*var)(TYPE a, TYPE b);
					 //		 //
					 //	int ptr = 0; char callConv = '\0';
					 //	g1:
					 //	_s += s.Length;
					 //	g2:
					 //	_SkipSpaceAndRN();
					 //	if(_IsCharIdentStart(*_s)) {
					 //		s = _GetIdent(_s);
					 //		_Symbol x2;
					 //		if(_sym.TryGetValue(s, out x2) && x2.symType == _SymT.Keyword) {
					 //			var k2 = x2 as _Keyword;
					 //			if(k2.symType == _SymT.CppType) _Err("unexpected");
					 //			if(k2.type == _KeywordT.Ignore) goto g1;
					 //			if(k2.type == _KeywordT.CallConv) {
					 //				char* cc = _s + 1; if(*cc == '_') cc++;
					 //				callConv = *cc;
					 //				goto g1;
					 //			}
					 //			//can be: __declspec, operator, 
					 //			_Err("unexpected");
					 //		} else { //now should be function name or variable name
					 //			_Err($"type {s}"); //TODO
					 //		}
					 //	} else if(*_s == '*' || *_s == '&') { //todo: use _TokIsPtr
					 //		ptr++;
					 //		_s++; goto g2;
					 //	} else if(*_s == '(') { //function type variable
					 //		_Err("not impl"); //TODO

				//	} else {
				//		_Err("unexpected");
				//	}
			}
		}

		/// <summary>
		/// Converts simple typedef or function typedef declaration.
		/// If simple typedef, calls _DeclareTypedef(x).
		/// _i must be after the x token. Finally it will be at semicolon.
		/// </summary>
		/// <param name="t">The referenced type, or function return type.</param>
		void _DeclareTypedefTypeOrFunc(_Symbol t, bool isConst)
		{
			int iTokType = _i - 1;

			//is function?
			bool isFunction = false;
			int i = _i, ptr = 0;
			for(;;) {
				char c = *T(i++);
				if(c == '*' || c == '&') ptr++;
				else {
					isFunction = _DetectIsFuncType(i - 1);
					break;
				}
			}

			if(isFunction) {
				_i = i - 1;
				_DeclareTypedefFunc(iTokType, t, ptr, isConst);
			} else {
				_DeclareTypedef(t);
			}
		}

		/// <summary>
		/// Converts C++ function type to C# delegate.
		/// _i must be at the first '(' (after typename and * etc). Finally it will be at semicolon.
		/// </summary>
		/// <param name="iTokType">Typename token.</param>
		/// <param name="t">Return type.</param>
		/// <param name="ptr">Pointer level.</param>
		/// <param name="isConst">Is const.</param>
		/// <param name="inl">Use for inline function type definition.</param>
		void _DeclareTypedefFunc(int iTokType, _Symbol t, int ptr, bool isConst, _INLINEFUNCTYPEDATA inl = null)
		{
			//return type
			string type = _ConvertTypeName(iTokType, t, ref ptr, false, isConst);

			bool isParen = _TokIsChar(_i, '('); //'([callConv]*[funcTypeOrVariable])(', else [callConv] 'funcType('
			if(isParen) _i++;

			//calling convention
			int iCallConv = 0;
			if(isParen) {
				if(_TokIsIdent(_i)) iCallConv = _i++;
				if(!_TokIsChar(_i, '*')) _Err(_i, "unexpected"); //todo: maybe also can be & (use _TokIsPtr)
				_i++;
			} else {
				if(_TokIsIdent(_i + 1)) iCallConv = _i++;
			}
			string callConv = iCallConv != 0 ? _ConvertCallConv(iCallConv) : "Cdecl";

			//name
			int iName = _i;
			string name = null;
			if(inl == null) {
				if(_SymbolExists(_i)) _Err(_i, "name already exists");
				name = _tok[_i].ToString();
			} else {
				if(_TokIsIdent(_i)) inl.paramName = _tok[_i].ToString();
				else { _i--; inl.paramName = "noname" + inl.paramIndex; }

				inl.typeName = name = (inl.parentName + "_" + inl.paramName);
			}
			_i++;
			if(isParen) {
				if(!_TokIsChar(_i, ')')) _Err(_i, "unexpected");
				_i++;
			}

			//start formatting
			StringBuilder sb = inl != null ? inl.sb : _sbDelegate;
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
		/// Converts simple typedef declaration.
		/// Scans the list of aliases and declares all.
		/// Starts from token index _i, which must at the first alias or * in the list. Finally it will be at semicolon.
		/// </summary>
		/// <param name="aliasOf">The type for which are declared aliases.</param>
		/// <param name="iTagName">Tag token index when 'typedef struct/etc tag {...} tail'. Else 0.</param>
		void _DeclareTypedef(_Symbol aliasOf, int iTagName = 0)
		{
			int ptr = 0; //0 - start or after comma, >0 pointer level, <0 after identifier
			int ptrBase = 0;
			int iStart = _i;

			//don't declare typedef to typedef
			if(iTagName == 0 && aliasOf is _Typedef) {
				var t = aliasOf as _Typedef;
				aliasOf = t.aliasOf;
				ptrBase = t.ptr;
			}

			for(; ; _i++) {
				if(_TokIsIdent(_i)) {
					if(ptr < 0) goto ge;
					bool dontAdd = iTagName > 0 && (_i == iTagName || _tok[_i].Equals(_tok[iTagName]));
					if(!dontAdd) {
						if(iTagName == 0 && _TokIsChar(_i + 1, '[')) { //typedef X Y[n];
							_AddSymbol(_i, new _Struct(false));
							string name = _tok[_i++].ToString();
							string type = _ConvertTypeName(iStart - 1, aliasOf, ref ptr, false, false);
							string attr;
							_i = _CArrayToMarshalAsAttribute(_i, out attr) - 1;
							_sbStruct.AppendFormat("public struct {0} {{\r\n\t{2} {1}[] a;\r\n}}\r\n", name, type, attr);
							//info:
							//SDK has 6 such typedefs, 2 of them are [1] ie used as variable-length array.
							//We convert to struct, because would be difficult to have this as typedef.
						} else {
							_AddSymbol(_i, new _Typedef(aliasOf, ptr + ptrBase, iTagName != 0));
						}
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
			//TODO: support array, eg typedef short KEYARRAY[128];
		}

		/// <summary>
		/// Scans tail of 'typedef struct etc {...} tail;'.
		/// Starts from token index _i, which must be after }. Finally it will be at semicolon.
		/// Returns token index of first found non-pointer alias.
		/// Returns 0 if there is no non-pointer alias.
		/// </summary>
		int _FindTypedefOfTag()
		{
			bool isPtr = false;
			for(int i = _i; ; i++) {
				if(_TokIsIdent(i)) {
					if(!isPtr) return i;
				} else {
					switch(*T(i)) {
					case '*':
					case '&':
						isPtr = true;
						break;
					case ',':
						isPtr = false;
						break;
					case ';':
						return 0;
					case '\0':
						_Err(i, "unexpected");
						break;
					}
				}
			}
		}

		/// <summary>
		/// Converts enum declaration.
		/// _i must be after 'enum'. Finally it will be at semicolon.
		/// </summary>
		/// <param name="isTypedef">Is 'typedef enum ...{...}X...;'.</param>
		void _DeclareEnum(bool isTypedef)
		{
			//scoped?
			if(_TokIs(_i, "class") || _TokIs(_i, "struct")) _i++;

			//name
			int iName = 0;
			bool nameless = false;
			if(_TokIsIdent(_i)) {
				iName = _i++;

				//is forward declaration? Rare but allowed.
				if(_TokIsChar(_i, ';')) {
					_AddSymbol(iName, new _Enum(true));
					return;
				}
			} else nameless = !isTypedef;

			//base type
			string sBaseType = null;
			if(_TokIsChar(_i, ':')) {
				_Symbol baseType = _FindType(++_i);
				int ptr;
				if(_Unalias(_i, ref baseType, out ptr) != _SymT.CppType || ptr != 0) _Err(_i, "unexpected");
				sBaseType = (baseType as _CppType).csType;
				_i++;
			}

			//body
			if(!_TokIsChar(_i, '{')) _Err(_i, "unexpected");
			int iBodyStart = _i++;

			if(nameless && _TokIsIdent(_i)) {
				//borrow enum name from first member. Benefits:
				//  Easier for users to find the enum.
				//  Will not be a name conflict.
				//  Easy.
				//Few enum in SDK are nameless.
				iName = _i;
				nameless = false;
			}

			for(;;) {
				char* s = T(_i++);
				if(*s == '}') break;
				if(*s == '\0') _Err(iBodyStart, "no }");
				if(sBaseType == null && *s == '0' && (s[1] == 'x' || s[1] == 'X')) sBaseType = "uint";
			}
			int iBodyEnd = _i;

			Debug.Assert(!nameless);
			if(nameless) return;

			if(iName == 0) { //info: isTypedef now is true
				iName = _FindTypedefOfTag();
				if(iName == 0) _Err(_i, "no name");
			}

			string name = _tok[iName].ToString();
			if(sBaseType == "uint") _sbEnum.AppendLine("[Flags]");
			_sbEnum.AppendFormat("public enum {0} {1}{2}\r\n",
				name,
				sBaseType == null ? "" : $":{sBaseType} ",
				new string(T(iBodyStart), 0, (int)(T(iBodyEnd - 1) + 1 - T(iBodyStart)))
				);

			_Enum x;
			if(_TryFindSymbolOfType(iName, out x) && x.forwardDecl == true) x.forwardDecl = false;
			else _AddSymbol(iName, x = new _Enum(false));

			if(isTypedef) {
				_DeclareTypedef(x, iName);
			}
		}

		/// <summary>
		/// Converts struct declaration.
		/// _i must be after 'struct'. Finally it will be at semicolon.
		/// </summary>
		/// <param name="isTypedef">Is 'typedef struct ...{...}X...;'.</param>
		void _DeclareStruct(bool isTypedef)
		{
			//__declspec etc
			string uuid = null;
			while(_TokIs(_i, "__declspec")) {
				if(!_TokIsChar(++_i, '(')) _Err(_i, "unexpected");
				if(_TokIs(_i + 1, "uuid") && _TokIsChar(_i + 2, '(') && _TokIsChar(_i + 3, '\"') && _TokIsChar(_i + 4, ')') && _TokIsChar(_i + 5, ')')) {
					uuid = _tok[_i + 3].ToString();
					_i += 6;
				} else {
					_i = _SkipEnclosed(_i) + 1;
				}
			}

			//name
			int iName = 0;
			if(_TokIsIdent(_i)) {
				iName = _i++;

				//is forward declaration?
				if(_TokIsChar(_i, ';')) {
					_AddSymbol(iName, new _Struct(true));
					return;
				}
			}

			__sbMembers.Clear();

			//base type
			if(_TokIsChar(_i, ':')) {
				_i++;
				if(_TokIsIdent(_i + 1) && _FindKeyword(_i).kwType == _KeywordT.PubPrivProt) _i++;

				for(; ; _i++) { //support multiple inheritance
					_Symbol baseType = _FindType(_i);
					int ptr;
					if(_Unalias(_i, ref baseType, out ptr) != _SymT.Struct || ptr != 0 || baseType.forwardDecl) _Err(_i, "unexpected");
					var b = baseType as _Struct;
					char[] baseMembers = new char[b.membersLength];
					_sbStruct.CopyTo(b.membersOffset, baseMembers, 0, b.membersLength);
					__sbMembers.Append(baseMembers);
					if(!_TokIsChar(++_i, ',')) break;
				}
			}

			//body
			if(!_TokIsChar(_i, '{')) _Err(_i, "unexpected");
			int iBodyStart = _i++;
			_INLINEFUNCTYPEDATA f = null;
			string access = "public";

			for(int iMember = 0; !_TokIsChar(_i, '}'); _i++) {

				if(_TokIsChar(_i + 1, ':') && 0 != _TokIs(_i, "public", "private", "protected")) {
					access = _tok[_i].ToString();
					_i += 2;
				}

				_PARAMDATA t;
				_i = _ParseParamOrMember(true, _i, out t, ref f, "\x5", iMember);

				for(;;) { //support TYPE a, b;
					switch(*T(_i)) {
					case ';': case ',': break;
					case '\0': _Err(iBodyStart, "no }"); break;
					default: _Err(_i, "unexpected"); break;
					}

					if(t.attributes != null) __sbMembers.AppendFormat("\t{0}\r\n", t.attributes);
					__sbMembers.AppendFormat("\t{2} {0} {1};\r\n", t.typeName, t.name, access);
					if(!_TokIsChar(_i, ',')) break;
					_i++;

					//ignore *. Assume never will be 'TYPE *a, b;' or 'TYPE a, *b;.
					while(_TokIsChar(_i, '*', '&')) _i++;
					//int ptr = 0;
					//while(_TokIsChar(_i, '*', '&')) { _i++; ptr++; }
					//Out(ptr); //TODO

					if(!_TokIsIdent(_i)) _Err(_i, "unexpected");
					t.name = _tok[_i++].ToString();

					if(_TokIsChar(_i, '[')) {
						_i = _CArrayToMarshalAsAttribute(_i, out t.attributes);
						if(!t.typeName.EndsWith_("[]")) t.typeName += "[]";
					} else if(t.attributes != null) {
						t.attributes = null;
						if(t.typeName.EndsWith_("[]")) t.typeName = t.typeName.Remove(t.typeName.Length - 2);
					}
				}

				//if(t.isUnsafe) isUnsafe = true; //TODO

				iMember++;
			}
			_i++;

			if(iName == 0) { //info: isTypedef now is true
				iName = _FindTypedefOfTag();
				if(iName == 0) _Err(_i, "no name");
			}

			_Struct x;
			if(_TryFindSymbolOfType(iName, out x) && x.forwardDecl == true) {
				x.forwardDecl = false;
			} else {
				x = new _Struct(false);
				_AddSymbol(iName, x);
			}

			string name = _tok[iName].ToString();
			string members = __sbMembers.ToString();
			bool replaceNamePlaceholders = members.IndexOf('\x5') >= 0;
			if(replaceNamePlaceholders) members = members.Replace("\x5", name);

			_sbStruct.Append("public struct ");
			_sbStruct.Append(name);
			_sbStruct.Append(" {\r\n");

			//get members offset and length in case this will be a base of another struct
			x.membersOffset = _sbStruct.Length;
			x.membersLength = members.Length;

			_sbStruct.Append(members);
			_sbStruct.Append("}\r\n");
			//TODO: unsafe

			if(isTypedef) {
				_DeclareTypedef(x, iName);
			}

			if(f != null) {
				string delegates = f.sb.ToString();
				if(replaceNamePlaceholders) delegates = delegates.Replace("\x5", name);
				_sbDelegate.Append(delegates);
			}
		}
		StringBuilder __sbMembers = new StringBuilder();

		void _DeclareUnion(bool isTypedef)
		{
			_Err(_i, "not impl"); //TODO

		}

		void _DeclareClass(bool isTypedef)
		{
			_Err(_i, "not impl"); //TODO

		}

		void _DeclareInterface(bool isTypedef)
		{
			_Err(_i, "not impl"); //TODO

		}
	}
}
