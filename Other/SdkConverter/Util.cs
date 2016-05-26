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
		#region SYMBOL

		/// <summary>
		/// If x is _Typedef, replaces it with its deepest used type.
		/// Error if x is not a type (is _Keyword).
		/// Returns symbol type of the final x type.
		/// </summary>
		/// <param name="iTok">Used only for error place.</param>
		/// <param name="ptr">Receives pointer level if it is a pointer typedef, else 0.</param>
		[DebuggerStepThrough]
		_SymT _Unalias(int iTok, ref _Symbol x, out int ptr)
		{
			ptr = 0;
			var t = x as _Typedef;
			if(t != null) {
				x = t.aliasOf;
				ptr = t.ptr;
			} else if(x is _Keyword) _Err(iTok, "unexpected");
			return x.symType;
		}

		/// <summary>
		/// Adds x to _sym.
		/// </summary>
		/// <param name="iTokName">Token index of symbol name.</param>
		/// <param name="x"></param>
		[DebuggerStepThrough]
		void _AddSymbol(int iTokName, _Symbol x)
		{
			__AddSymbol(_tok[iTokName], x, iTokName);
		}

		/// <summary>
		/// Adds x to _sym.
		/// </summary>
		/// <param name="iTokError">Where to show error if need.</param>
		[DebuggerStepThrough]
		void _AddSymbol(string name, _Symbol x, int iTokError)
		{
			fixed (char* n = name) { __AddSymbol(new _Token(n, name.Length), x, iTokError); }
		}

		/// <summary>
		/// Adds x to _sym.
		/// </summary>
		/// <param name="iTokError">Where to show error if need.</param>
		[DebuggerStepThrough]
		void __AddSymbol(_Token name, _Symbol x, int iTokError)
		{
			//Out(name);
			try {
				_sym.Add(name, x);
			} catch(ArgumentException) {
				_Symbol p = _sym[name];
				if(x.symType != p.symType) _Err(iTokError, "name already exists");
				if(!x.forwardDecl) {
					if(x.symType != _SymT.Typedef && x.symType != _SymT.TypedefFunc) _Err(iTokError, "already defined");
					//info: C++ allows multiple identical typedef. We don't check identity, it is quite difficult, assume the header file is without such errors.
					//info: before adding struct or enum, the caller checks whether it is forward-declaraed. If so, overwrites the forward-declared object and does not call this function, because cannot change object associated with that name because forward-declared object pointer can be used in a typedef.
				}
			}
		}

		/// <summary>
		/// Finds symbol of token iTok in _sym.
		/// Error if not found or the token is not an identifier.
		/// </summary>
		[DebuggerStepThrough]
		_Symbol _FindSymbol(int iTok)
		{
			_Symbol x;
			if(_sym.TryGetValue(_tok[iTok], out x)) return x;
			_Err(iTok, _TokIsIdent(iTok) ? "unknown identifier" : "unexpected");
			return null;
		}

		/// <summary>
		/// Finds symbol of token iTok in _sym.
		/// Error if the token is not an identifier.
		/// </summary>
		[DebuggerStepThrough]
		bool _TryFindSymbol(int iTok, out _Symbol x)
		{
			if(_sym.TryGetValue(_tok[iTok], out x)) return true;
			if(!_TokIsIdent(iTok)) _Err(iTok, "unexpected");
			return false;
		}

		/// <summary>
		/// Finds symbol of T type of token iTok in _sym.
		/// Error if the token is not an identifier.
		/// </summary>
		[DebuggerStepThrough]
		bool _TryFindSymbolOfType<T>(int iTok, out T x) where T : _Symbol
		{
			x = null;
			_Symbol t;
			if(!_TryFindSymbol(iTok, out t)) return false;
			x = t as T;
			return x != null;
		}

		/// <summary>
		/// Finds symbol of token iTok in _sym.
		/// Error if not found or not a keyword or the token is not an identifier.
		/// </summary>
		[DebuggerStepThrough]
		_Keyword _FindKeyword(int iTok)
		{
			_Symbol x = _FindSymbol(iTok);
			if(x.symType != _SymT.Keyword) _Err(iTok, "unexpected");
			return x as _Keyword;
		}

		/// <summary>
		/// Finds symbol of token iTok in _sym.
		/// Error if not found or not a type (struct, enum etc) or the token is not an identifier.
		/// </summary>
		[DebuggerStepThrough]
		_Symbol _FindType(int iTok)
		{
			_Symbol x = _FindSymbol(iTok);
			if(x.symType == _SymT.Keyword) _Err(iTok, "unexpected");
			return x;
		}

		/// <summary>
		/// Returns true if symbol of token iTok exists in _sym.
		/// Error if the token is not an identifier.
		/// </summary>
		[DebuggerStepThrough]
		bool _SymbolExists(int iTok)
		{
			if(_sym.ContainsKey(_tok[iTok])) return true;
			if(!_TokIsIdent(iTok)) _Err(iTok, "unexpected");
			return false;
		}

		#endregion

		#region TOKEN

		/// <summary>
		/// Returns _tok[i].s.
		/// </summary>
		[DebuggerStepThrough]
		char* T(int i) { return _tok[i].s; }

		/// <summary>
		/// Returns true if token iTok string is equal to s.
		/// </summary>
		[DebuggerStepThrough]
		bool _TokIs(int iTok, string s)
		{
			return _tok[iTok].Equals(s);
		}

		/// <summary>
		/// Returns 1-based index of matching string at token iTok.
		/// </summary>
		[DebuggerStepThrough]
		int _TokIs(int iTok, params string[] a)
		{
			for(int i = 0; i < a.Length; i++) if(_tok[iTok].Equals(a[i])) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if token iTok is an identifier.
		/// </summary>
		[DebuggerStepThrough]
		bool _TokIsIdent(int iTok)
		{
			return _IsCharIdentStart(*T(iTok));
		}

		/// <summary>
		/// Returns true if token iTok is character c.
		/// </summary>
		[DebuggerStepThrough]
		bool _TokIsChar(int iTok, char c)
		{
			return *T(iTok) == c;
		}

		/// <summary>
		/// Returns true if token iTok is character c1 or c2.
		/// </summary>
		[DebuggerStepThrough]
		bool _TokIsChar(int iTok, char c1, char c2)
		{
			char c = *T(iTok);
			return c == c1 || c == c2;
		}

		/// <summary>
		/// Returns true, if at token iTokAfterTypeNameAndPtr is '([callConv]*[funcTypeOrVariable])(' or [callConv] 'funcType('.
		/// </summary>
		bool _DetectIsFuncType(int iTokAfterTypeNameAndPtr)
		{
			int i = iTokAfterTypeNameAndPtr;
			if(_TokIsChar(i, '(')) {
				if(_TokIsIdent(++i)) i++; //eg __stdcall
				if(!_TokIsChar(i, '*')) return false;
				if(_TokIsIdent(++i)) i++; //func type name or parameter/member name (optional)
				if(!_TokIsChar(i, ')')) return false;
				return _TokIsChar(i + 1, '(');
			} else {
				if(!_TokIsIdent(i)) return false; //can be funcType or callConv
				if(_TokIsChar(i + 1, '(')) return true;
				if(!_TokIsChar(i + 2, '(') || !_TokIsIdent(i + 1)) return false;
				_Keyword k;
				return _TryFindSymbolOfType(i, out k) && k.kwType == _KeywordT.CallConv;
			}
		}

		/// <summary>
		/// Skips code block enclosed in {}, (), [] or ˂˃, starting from token iTok, which must be at the starting { etc.
		/// Returns token index of the ending } etc.
		/// Inside the block:
		///		Skips nested enclosed blocks of the same type.
		///		Ignores nested enclosed blocks of other types, eg (), [] etc blocks when this block is {}.
		/// Error if iTok character is not {([˂.
		/// Error if the ending })]˃ is missing.
		/// </summary>
		[DebuggerStepThrough]
		int _SkipEnclosed(int iTok)
		{
			char cStart = *T(iTok);
			if(!(cStart == '{' || cStart == '(' || cStart == '[' || cStart == '<')) _Err(iTok, "unexpected");

			char cEnd = cStart; cEnd++; if(cEnd != ')') cEnd++; //')' is '(' + 1, others + 2
			int level = 1;

			for(int i = iTok + 1; ; i++) {
				char c = *T(i);
				if(c == cStart) {
					level++;
				} else if(c == cEnd) {
					level--;
					if(level == 0) return i;
				} else if(c == 0) {
					_Err(iTok, $"missing {cEnd}");
				}
			}
		}

		#endregion

		#region STRING

		/// <summary>
		/// Parses and folds string constant, starting from s, which must be at the starting ".
		/// Returns folded string length, including both ".
		/// For example, if s is '"a" "b"', makes it '"ab"   ' and returns 4.
		/// Error if the ending " is missing.
		/// </summary>
		[DebuggerStepThrough]
		int _SkipString(char* s)
		{
			Debug.Assert(*s == '\"');
			char* s0 = s++, d = s; //d used for folding
			g0:
			for(; ; s++) {
				*d++ = *s; //folding
				if(*s == '\"') {
					if(s[-1] == '\\') {
						//is \" or \\"?
						int k = -1; while(s[k - 1] == '\\') k--;
						if((k & 1) != 0) continue;
					}
					break;
				}
				if(*s <= '\r') {
					if(*s == 0 || *s == '\r' || *s == '\n') _Err(s0, "missing \"");
				}
			}

			//fold strings
			char* f = ++s; //skip "
			while(_IsCharSpaceOrRN(*f)) f++;
			if(*f == '\"') { s = f + 1; d--; goto g0; } //if string again
			for(f = d; f < s; f++) *f = ' '; //erase what is moved to the left

			return (int)(d - s0);

			//info: could also give parsed length, but this is simpler. Caller will have to skip spaces in any case.
		}

		/// <summary>
		/// Parses character constant, starting from s, which must be at the starting '.
		/// Returns its length, including both '.
		/// Error if the ending ' is missing.
		/// </summary>
		[DebuggerStepThrough]
		int _SkipApos(char* s)
		{
			Debug.Assert(*s == '\'');
			for(char* s0 = s++; ; s++) {
				if(*s == '\'') {
					if(s[-1] == '\\') {
						//is \' or \\'?
						int k = -1; while(s[k - 1] == '\\') k--;
						if((k & 1) != 0) continue;
					}
					return (int)(s + 1 - s0);
				}
				if(*s <= '\r') {
					if(*s == 0 || *s == '\r' || *s == '\n') _Err(s0, "missing '");
				}
			}
		}

		/// <summary>
		/// Gets line length.
		/// Scans string until first '\r', '\n' or '\0'.
		/// </summary>
		[DebuggerStepThrough]
		static int _LenLine(char* s)
		{
			char* s0 = s;
			while(!_IsCharRN0(*s)) s++;
			return (int)(s - s0);
		}

		/// <summary>
		/// Gets identifier length.
		/// Returns 0 if s does not start with an identifier.
		/// </summary>
		[DebuggerStepThrough]
		static int _LenIdent(char* s)
		{
			if(!_IsCharIdentStart(*s)) return 0;
			char* s0 = s++;
			while(_IsCharIdent(*s)) s++;
			return (int)(s - s0);
		}

		/// <summary>
		/// Returns true if s starts with word and is not followed by a C++ identifier character.
		/// </summary>
		[DebuggerStepThrough]
		static bool _IsWord(char* s, string word)
		{
			int i = 0;
			for(; i < word.Length; i++) if(s[i] != word[i]) return false;
			return !_IsCharIdent(s[i]);
		}

		/// <summary>
		/// Returns true if s starts with prefix.
		/// </summary>
		[DebuggerStepThrough]
		static bool _IsPrefix(char* s, string prefix)
		{
			for(int i = 0; i < prefix.Length; i++) if(s[i] != prefix[i]) return false;
			return true;
		}

		/// <summary>
		/// Parses a number literal and returns its length, including suffix.
		/// Error if invalid.
		/// </summary>
		[DebuggerStepThrough]
		int _LenNumber(char* s)
		{
			if(!_IsCharDigit(*s)) return 0;
			char* s0 = s++;

			//hex, bin, oct
			int hexEtc = 0;
			if(*s0 == '0') {
				if(*s == 'x' || *s == 'X') {
					hexEtc = 2;
					for(++s; _IsCharHexDigit(*s); s++) { }
				} else if(*s == 'b' || *s == 'B') {
					hexEtc = 3;
					for(++s; *s == '0' || *s == '1'; s++) { }
				} else if(_IsCharDigit(*s)) hexEtc = 1; //or can be double like 01.2

				if(hexEtc >= 2 && (s - s0) == 2) _Err(s, "unexpected");
			}

			bool dbl = false;
			if(hexEtc < 2) {
				while(_IsCharDigit(*s)) s++;
				//double fraction
				if(*s == '.') {
					dbl = true;
					for(++s; _IsCharDigit(*s); s++) { }
				}
			}

			//double exponent
			if(*s == 'E' || *s == 'e') {
				dbl = true;
				s++;
				if(*s == '-' || *s == '+') s++;
				if(!_IsCharDigit(*s)) _Err(s, "unexpected");
				while(_IsCharDigit(*s)) s++;
			}

			if(hexEtc == 1 && !dbl) { //validate oct digits now
				for(char* t = s0 + 2; t < s; t++) if(*t > '7') _Err(t, "unexpected");
			}

			//suffix
			uint suffixType;
			s += _NumberSuffix(s, dbl, out suffixType);

			return (int)(s - s0);
		}

		/// <summary>
		/// Scans integer or floating-point constant suffix and returns its length.
		/// Error if invalid suffix.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="type">Receives flags: 1 unsigned, 2 __int64, 4 float.</param>
		[DebuggerStepThrough]
		int _NumberSuffix(char* s, bool floatingPoint, out uint type)
		{
			type = 0;
			char* s0 = s;

			if(floatingPoint) {
				if(*s == 'f' || *s == 'F') { s++; type |= 4; } else if(*s == 'l' || *s == 'L') s++;
			} else {
				if(*s == 'u' || *s == 'U') { s++; type |= 1; }

				if(*s == 'l' || *s == 'L') {
					s++;
					if(*s == 'l' || *s == 'L') { s++; type |= 2; }
				} else if(*s == 'i' || *s == 'I') {
					if(s[1] == '6' && s[2] == '4') type |= 2;
					else if(s[1] == '8') s--;
					else if(!((s[1] == '3' && s[2] == '2') || (s[1] == '1' && s[2] == '6'))) _Err(s, "unexpected");

					s += 3;
				}
			}

			if(_IsCharIdent(*s)) _Err(s, "unexpected");
			return (int)(s - s0);
		}

		#endregion

		#region CHARACTER
		/// <summary>
		/// Character type table for _IsCharIdent etc.
		/// </summary>
		static readonly byte[] _ctt = new byte[0x1000];

		static void _InitTables()
		{
			_ctt['_'] = 1; _ctt['$'] = 1;
			for(int i = 'a'; i <= 'z'; i++) _ctt[i] = 1;
			for(int i = 'A'; i <= 'Z'; i++) _ctt[i] = 1;
			for(int i = '0'; i <= '9'; i++) _ctt[i] = 2;
			_ctt[' '] = _ctt['\t'] = _ctt['\v'] = _ctt['\f'] = 4;
			_ctt['\r'] = _ctt['\n'] = 8;
			//_ctt['!'] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = 16;
			//_ctt['('] = _ctt[')'] = _ctt['{'] = _ctt['}'] = _ctt['['] = _ctt[']'] = _ctt[';'] = _ctt['\''] = _ctt[':'] = _ctt['\"'] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = _ctt[''] = 32;
			for(int i = 'a'; i <= 'f'; i++) _ctt[i] |= 64;
			for(int i = 'A'; i <= 'F'; i++) _ctt[i] |= 64;
			_ctt[0] = 128;
			//others 0
		}

		/// <summary>
		/// Returns true if c is a C++ identifier character: alphanumeric, '_', '$'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharIdent(char c)
		{
			return (_ctt[c] & 3) != 0;
		}

		/// <summary>
		/// Returns true if c is a C++ identifier start character: alpha, '_', '$'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharIdentStart(char c)
		{
			return (_ctt[c] & 1) != 0;
		}

		/// <summary>
		/// Returns true if c is a number digit '0' to '9'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharDigit(char c)
		{
			return (_ctt[c] & 2) != 0;
		}

		/// <summary>
		/// Returns true if c is a number digit '0' to '9'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharHexDigit(char c)
		{
			return (_ctt[c] & (2 | 64)) != 0;
		}

		/// <summary>
		/// Returns true if c is a C++ operator character.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharOperator(char c)
		{
			return (_ctt[c] & 16) != 0;
		}

		/// <summary>
		/// Returns true if c is a C++ punctuator character: .
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharPunctuator(char c)
		{
			return (_ctt[c] & 32) != 0;
		}

		/// <summary>
		/// Returns true if c is a C++ enclosing character: '{'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharEnclosing(char c)
		{
			return (_ctt[c] & 64) != 0;
		}

		/// <summary>
		/// Returns true if c is a C++ space character, including new line characters: ' ', '\t', '\r', '\n', '\v', '\f'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharSpaceOrRN(char c)
		{
			return (_ctt[c] & 12) != 0;
		}

		/// <summary>
		/// Returns true if c is a C++ space character, except new line characters: ' ', '\t', '\v', '\f'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharSpaceNoRN(char c)
		{
			return _ctt[c] == 4;
		}

		/// <summary>
		/// Returns true if c is a new line character: '\r', '\n'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharRN(char c)
		{
			return _ctt[c] == 8;
		}

		/// <summary>
		/// Returns true if c is a new line character or end of string: '\r', '\n', '\0'.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool _IsCharRN0(char c)
		{
			return (_ctt[c] & (8 | 128)) != 0;
		}

		#endregion

		#region ERROR

		/// <summary>
		/// Gets offset of token iTok in _src.
		/// </summary>
		int _Pos(int iTok) { return iTok < _tok.Count ? (int)(T(iTok) - _s0) : 0; }

		/// <summary>
		/// Throws exception indicating error at token iTok position.
		/// </summary>
		[DebuggerStepThrough]
		void _Err(int iTok, string errorText, [CallerMemberName] string callerName = null, [CallerFilePath] string callerFile = null, [CallerLineNumber] int callerLine = 0)
		{
			__ThrowConverterException(_Pos(iTok), errorText, callerName, callerFile, callerLine);
		}

		/// <summary>
		/// Throws exception indicating error at s position (s must be in _src).
		/// </summary>
		[DebuggerStepThrough]
		void _Err(char* s, string errorText, [CallerMemberName] string callerName = null, [CallerFilePath] string callerFile = null, [CallerLineNumber] int callerLine = 0)
		{
			__ThrowConverterException((int)(s - _s0), errorText, callerName, callerFile, callerLine);
		}

		[DebuggerStepThrough]
		void __ThrowConverterException(int pos, string errorText, string callerName, string callerFile, int callerLine)
		{
			throw new ConverterException(
				$"{errorText}\r\n\tin {callerName}, {Path.GetFileName(callerFile)}:({callerLine})"
				, pos);
		}

		#endregion

	}

	class ConverterException :Exception
	{
		public int Offset { get; private set; }

		public ConverterException(string s, int offset) : base(s)
		{
			Offset = offset;
#if TEST_SMALL
			//Offset += 3; //VS saves some files with BOM, and QM2 loads with that BOM, but we get file data without BOM
#endif
		}
	}
}
