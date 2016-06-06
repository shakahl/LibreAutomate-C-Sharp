//#define TEST_SMALL //in project Properties

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
		StringBuilder _sbFunc = new StringBuilder();
		StringBuilder _sbInterface = new StringBuilder();
		StringBuilder _sbCoclass = new StringBuilder();
		StringBuilder _sbComment = new StringBuilder();
		StringBuilder _sbInlineDelegate = new StringBuilder(); //from callback function types defined in parameter list or member list

		List<_Token> _tok = new List<_Token>();
		int _nTok; //token count, except the last '\0' tokens
		int _nTokUntilDefUndef; //token count until first `d or `u (was #define/#undef), except the separating '\0' token
		int _i; //current token

		_Namespace[] _ns = new _Namespace[10]; //stack of namespaces
		int _nsCurrent; //index in _ns
		StringBuilder _sbType; //_ns[_nsCurrent].sb
		Dictionary<_Token, _Symbol> _keywords = new Dictionary<_Token, _Symbol>();

		//Dictionary<string, string> _func = new Dictionary<string, string>(); //function declaration
		//List<string> _cppConst = new List<string>(); //const CONSTANT
		//List<string> _comment = new List<string>(); //added when cannot convert, eg #define MACRO(...)
		Dictionary<string, string> _defineConst = new Dictionary<string, string>(); //#define CONSTANT
		Dictionary<string, string> _defineOther = new Dictionary<string, string>(); //#define MACRO(...)

		Stack<int> _packStack = new Stack<int>(); int _pack = 8; //#pragma pack

		int _anonymousStructSuffixCounter; //used to create unique name for anonymous struct

		//Regex _rxIndent = new Regex("(?m)^(?=.)", RegexOptions.CultureInvariant);

		public void Convert(string hFile, /*( MMMMM* )*/ string csFile)
		{
			try {
				if(_src != null) throw new Exception("cannot call Convert multiple times. Create new Converter instance.");

				using(var reader = new StreamReader(hFile)) {
					long n = reader.BaseStream.Length + 4; if(n > int.MaxValue / 4) throw new Exception("file too big");
					reader.Read(_src = new char[n], 0, (int)n);
				}

				_InitTables();
				_InitSymbols();

				string stf = null;

				fixed (char* p = _src)
				{
					_s0 = p;
					_Tokenize(p);

					//Test(); return;

					_i = 1;
					_ConvertAll(0);

					//_ConvertTypedefAndTag();

					//Out("#define CONST:");
					//Out(_defineConst);
					//Out("#define other:");
					//Out(_defineOther);
					//OutList(_defineConst.Count, _defineOther.Count);
					//27890, 1061
					//27659, 998
					//27605, 983

					foreach(var v in _defineConst) {
						_sbConst.AppendLine(v.Value);
						//TODO: add empty line if previous prefix is different
					}
					foreach(var v in _defineOther) {
						_sbComment.AppendFormat("/// #define {0}{1}\r\nconst string {0} = \"C macro\";\r\n\r\n", v.Key, v.Value);
					}
					//TODO: finally remove '#define FuncX FuncY', '#define X UnknownIdentifier' etc.

					stf = _PostProcessTypesFunctionsInterfaces();
					//Out(stf);
				}

				string sh = @"// Windows API for C#.
// Converted commonly used Windows 10 SDK header files.

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

//add this to projects that will use these API
[module: DefaultCharSet(CharSet.Unicode)]

public static unsafe class API
{
";

				using(var writer = new StreamWriter(csFile)) {
					writer.Write(sh);
					writer.Write(stf);
					//if(_sbType.Length > 0 || _sbInterface.Length > 0) {
					//	writer.Write("\r\n// USED TAG-TYPES\r\n");
					//	if(_sbType.Length > 0) writer.Write(_sbType.ToString());
					//	if(_sbInterface.Length > 0) writer.Write(_sbInterface.ToString());
					//}
					writer.Write("\r\n// COCLASSES\r\n");
					writer.Write(_sbCoclass.ToString());
					writer.Write("\r\n// CONSTANTS\r\n\r\n");
					writer.Write(_sbConst.ToString());
					writer.Write("\r\n// CANNOT CONVERT\r\n\r\n");
					writer.Write(_sbComment.ToString());
					writer.Write("\r\n}\r\n");
				}
			}
			//#if !TEST_SMALL
			catch(ConverterException e) {
				Out(e);
				Wnd.FindCN("QM_Editor").SendS(Api.WM_SETTEXT, 1, $"M \"api_converter_error\" A(||) {e.Message}||{hFile}||{e.Offset}");
			}
			catch(Exception e) {
				Out(e);
				Wnd.FindCN("QM_Editor").SendS(Api.WM_SETTEXT, 1, $"M \"api_converter_error\" A(||) {" "}||{hFile}||{_Pos(_i)}");
			}
			//#endif
			finally {
				Marshal.FreeHGlobal((IntPtr)_keywordMemory);
			}

			Out("DONE");
		}

		char* _keywordMemory;
		char* _km; //curent keyword

		void _AddKeyword(string name, _Symbol sym)
		{
			char* n = _km;
			for(int i = 0; i < name.Length; i++) *_km++ = name[i];
			_keywords.Add(new _Token(n, (int)(_km - n)), sym);
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
			for(int i = 0; i < _ns.Length; i++) _ns[i] = new _Namespace();
			_sbType = _ns[0].sb;

			_km = _keywordMemory = (char*)Marshal.AllocHGlobal(4000);

			//Add C++ keywords and types, except those that can only be in function body or other places where we skip.
			//The commented keywords/types are currently not supported and not expected to be in C/C++ headers that we'll parse.

			_Keyword k;

			//_AddKeywords(new _Keyword(cannotStartStatement:true), "__based", "_based"); //0 in SDK
			//_AddKeyword("__event", new _Keyword()); //0 in SDK
			//_AddKeyword("__identifier", new _Keyword()); //0 in SDK
			//_AddKeywords(new _Keyword(), "__if_exists", "_if_exists", "__if_not_exists", "_if_not_exists"); //0 in SDK. Usually used only in function body.
			//_AddKeywords(new _Keyword(cannotStartStatement: true), "__multiple_inheritance", "_multiple_inheritance", "__single_inheritance", "_single_inheritance", "__virtual_inheritance", "_virtual_inheritance"); //0 in SDK
			//_AddKeyword("__uuidof", new _Keyword(cannotStartStatement: true)); //should be only in function body
			//_AddKeyword("alignas", k = new _Keyword(cannotStartStatement: true)); //removed in script
			//_AddKeywords(new _Keyword(cannotStartStatement: true), "alignof", "__alignof"); //should be only in function body
			//_AddKeywords(new _Keyword(cannotStartStatement: true)"false", "true"); //should be only in ignored code
			//_AddKeyword("property", k = new _Keyword()); //0 in SDK
			//_AddKeyword("signed", k = new _Keyword()); //replaced in script
			//_AddKeywords(new _Keyword(cannotStartStatement: true), "throw", "noexcept"); //eg void f() throw(int); //will be skipped together with functions
			//_AddKeyword("unsigned", k = new _Keyword()); //replaced in script
			//_AddKeyword("nullptr", k); //0 in SDK

			k = new _Keyword(cannotStartStatement: true);
			_AddKeyword("sizeof", k); //can be like 'member[sizeof(X)]' or '#define X sizeof(Y)'

			k = new _Keyword();
			_AddKeyword("const", k);
			_AddKeyword("extern", k);
			_AddKeyword("namespace", k); //0 in SDK (1 in CRT, which is removed)
			_AddKeyword("using", k);
			_AddKeyword("virtual", k);

			k = new _Keyword(_KeywordT.TypeDecl);
			_AddKeyword("__interface", k);
			_AddKeyword("class", k);
			_AddKeyword("enum", k);
			_AddKeyword("struct", k);
			_AddKeyword("typedef", k);
			_AddKeyword("union", k);

			_AddKeywords(new _Keyword(_KeywordT.CallConv, cannotStartStatement: true), "__cdecl", "_cdecl", "__fastcall", "_fastcall", "__stdcall", "_stdcall", "__thiscall");

			_AddKeywords(new _Keyword(_KeywordT.IgnoreFuncEtc), "__forceinline", "__inline", "_inline", "inline", "template", "operator", "static_assert");

			_AddKeywords(new _Keyword(_KeywordT.PubPrivProt), "private", "protected", "public");

			//_AddKeywords(new _Keyword(_KeywordT.Ignore), "__unaligned", "__w64", "_w64", "volatile", "explicit", "friend", "mutable", "static", and so on); //removed in script
			//_AddKeywords(k = new _Keyword(_KeywordT.Declspec), "__declspec", "_declspec"); //removed in script

			_AddKeyword("uuid", k = new _Keyword(cannotStartStatement: true)); //was '__declspec(uuid', replaced in script
			_AddKeyword("guid", k = new _Keyword()); //was 'extern "C" const GUID  GUID_MAX_POWER_SAVINGS = { 0x...};', replaced in script

			_AddKeywords(new _CppType("sbyte", 1, false), "__int8", "char");
			_AddKeywords(new _CppType("short", 2, false), "__int16", "short");
			_AddKeywords(new _CppType("int", 4, false), "__int32", "int", "long");
			_AddKeyword("__int64", new _CppType("long", 8, false));
			_AddKeyword("double", new _CppType("double", 8, false));
			_AddKeyword("float", new _CppType("float", 4, false));
			_AddKeywords(new _CppType("char", 2, true), "__wchar_t", "wchar_t", "char16_t");
			//our preprocessor script replaced 'unsigned type' to 'u$type'
			//also the script replaced 'signed type' to 'type, 'long long' to '__int64', 'long double' to 'double'
			_AddKeywords(new _CppType("byte", 1, true), "u$__int8", "u$char");
			_AddKeywords(new _CppType("ushort", 2, true), "u$__int16", "u$short");
			_AddKeywords(new _CppType("uint", 4, true), "u$__int32", "u$int", "u$long", "char32_t");
			_AddKeyword("u$__int64", new _CppType("ulong", 8, true));
			_AddKeyword("bool", new _CppType("bool", 1, true));
			_AddKeyword("void", new _CppType("void", 0, false));
			//_AddKeywords(new _Keyword(), "__m128", "__m128d", "__m128i"); //our script defined these as struct, because C# does not have a matching type
			//_AddKeywords(new _Keyword(), "__m64"); //not used in SDK
			_AddKeyword("auto", new _CppType("var", 1, false)); //has two meanings depending on compiler options. The old auto is a local variable, and we will not encounter it because we skip function bodies. The new auto is like C# var; it can also be applied to global variables too, unlike in C#; we'll ignore all variable declarations.
			_AddKeyword("IntPtr", new _CppType("IntPtr", 8, false));
		}

		void _Tokenize(char* s)
		{
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
				case '`': //was #define, #undef or #pragma pack
					if(isNewLine) { //else it is preprocessor operator
						if(_nTokUntilDefUndef == 0 && s[1] != '(') {
							_nTokUntilDefUndef = _tok.Count;
							s[-1] = '\0'; //was '\n
							_tok.Add(new _Token(s - 1, 1));
						}
					}
					_tok.Add(new _Token(s, 1));
					break;
				default:
					//TODO: replace '$' to '_'. Although SDK does not have such identifiers when removed CRT headers.
					if(_IsCharIdentStart(c)) {
						len = _LenIdent(s);

						//is prefix"string" or prefix'char'?
						bool isPrefix = false;
						switch(s[len]) {
						case '\"':
							if(len == 1) isPrefix = (c == 'L' || c == 'u' || c == 'U');
							else if(len == 2) isPrefix = (c == 'u' && s[1] == '8');
							//info: raw strings replaced to escaped strings when preprocessing
							break;
						case '\'':
							if(len == 1) isPrefix = (c == 'L' || c == 'u' || c == 'U');
							break;
						}
						if(isPrefix) { //remove prefix
							for(; len > 0; len--) *s++ = ' ';
							s--;
							continue;
						}

					} else if(_IsCharDigit(c)) len = _LenNumber(s);
					else if(c == '\"') len = _SkipString(s);
					else if(c == '\'') len = _SkipApos(s);
					else len = 1;
					_tok.Add(new _Token(s, len));
					s += len - 1;
					break;
				}
			}

			_nTok = _tok.Count;
			if(_nTokUntilDefUndef == 0) _nTokUntilDefUndef = _nTok;

			for(int i = 0; i < 10; i++) _tok.Add(new _Token(s, 1)); //to safely query next token
			_tok[0] = new _Token(s, 1); //to safely query previous token. Also used as an empty token. The empty list item added before tokenizing.
		}

		void _ConvertAll(int nestLevel)
		{
			for(; _i < _nTokUntilDefUndef; _i++) {
				//Out(_TokToString(_i));
				char* s = T(_i); char c = *s;
				if(_IsCharIdentStart(c)) {
					_Statement();
				} else if(c == '`') { //was #pragma pack
					_PragmaPack();
				} else if(c != ';' && c != '}') {
#if TEST_SMALL
					if(c == '/' && s[1] == '/') return;
#endif
					//Out(_i);
					_Err(_i, $"unexpected: {_TokToString(_i)}");
				}
			}

			//info: script converted all #define/#undef to `d/`u and placed at the end
			_i++; //skip '\0' token inserted before first `d/`u token
			for(; _i < _nTok; _i++) {
				char* s = T(_i);
				if(*s == '`') { //was #define, #undef
					_DefineUndef();
				} else {
#if TEST_SMALL
					if(*s == '/' && s[1] == '/') return;
#endif
					//Out(_i);
					_Err(_i, $"unexpected: {_TokToString(_i)}");
				}
			}
		}

		//bool _debugFARPROC;

		void _Statement()
		{
			g0:
			_Symbol x = _FindSymbol(_i, true);
			var k = x as _Keyword;
			if(k != null) {
				if(k.cannotStartStatement) _Err(_i, "unexpected");

				if(k.kwType == _KeywordT.TypeDecl) {
					//is forward decl like 'struct X* Func(...);'?
					if(!_TokIsChar(_i, 't') && _TokIsChar(_i + 2, '*', '&')) {
						_InlineForwardDeclaration();
						goto g0;
					}

					var ftd = new _FINDTYPEDATA(); //just for ref parameter
					_DeclareType(ref ftd, true);
				} else if(k.kwType == _KeywordT.IgnoreFuncEtc) {
					_SkipStatement();
					return;
				} else if(_TokIs(_i, "extern")) {
					if(_TokIsChar(++_i, '\"')) { //extern "C"
						if(_TokIsChar(_i + 1, '{')) _i++;
						return;
					} else { //extern T X
						_SkipStatement(true);
						//_Err(_i, "stop");
					}
				} else if(_TokIs(_i, "guid")) { //script converted DEFINE_GUID(X) to C# declaration with 'guid ' prefix
					char* s0 = T(++_i);
					while(!_TokIsChar(_i, ';')) _i++;
					_sbConst.AppendLine(new string(s0, 0, (int)(T(_i) + 1 - s0)));
				} else if(_TokIs(_i, "const")) {
					_SkipStatement(true);
					//if(!_Const()) _Err(_i, "unexpected");
				} else {
					//can be:
					//namespace



					_Err(_i, "unexpected"); //TODO
				}
				if(!_TokIsChar(_i, ';')) _Err(_i, "unexpected");

			} else { //a type
				_SkipStatement();
				//_Err(_i, "not impl");
				//		 //now we expect one of:
				//		 //	function declaration, like TYPE __stdcall Func(TYPE a, TYPE b);
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
				//			//can be: operator, 
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
		/// Skips current statement.
		/// _i can be at any place in the statement except in parts enclosed in (), [] or {}.
		/// Finally _i will be at the ending ';' or '}' (if '}' is not followed by ';').
		/// </summary>
		/// <param name="debugShow"></param>
		void _SkipStatement(bool debugShow = false)
		{
			//#if DEBUG
			//			int i0 = _i;
			//#endif
			gk1:
			while(!_TokIsChar(_i, "({;[")) _i++;
			if(!_TokIsChar(_i, ';')) {
				_SkipEnclosed();
				if(!_TokIsChar(_i, '}')) goto gk1; //skip any number of (enclosed) or [enclosed] parts, else skip single {enclosed} part
				if(_TokIsChar(_i + 1, ';')) _i++;
			}
			//#if DEBUG
			//			if(debugShow) {
			//				string s = new string(T(i0), 0, (int)(T(_i + 1) - T(i0)));
			//				Out($"<><c 0xff>skipped:</c>\r\n{s}");
			//			}
			//#endif
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

		bool _Const()
		{
			return false;
			_i++;
			if(!(_TokIsChar(_i + 3, '}') && _TokIsChar(_i + 2, '=') && _TokIsIdent(_i) && _TokIsIdent(_i + 1))) return false;

			return true;
		}
	}
}
