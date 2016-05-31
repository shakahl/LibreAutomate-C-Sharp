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
		enum _KeywordT :byte
		{
			Any,
			Normal,
			TypeDecl, //enum, struct, union, class, __interface, typedef
			CallConv, //__stdcall etc
			PubPrivProt, //public, private, protected
			//Declspec, //__declspec(...) //removed in script
			//Ignore, //volatile etc //removed in script
			IgnoreFuncEtc, //inline, template, operator, static_assert
		}

		[DebuggerStepThrough]
		class _Namespace
		{
			/// <summary>
			/// Symbols of current namespace. Types, typedef etc. Not C++ keywords/types.
			/// </summary>
			public Dictionary<_Token, _Symbol> sym = new Dictionary<_Token, _Symbol>();

			/// <summary>
			/// Formats members and nested types of current namespace. Cleared/reused for each type.
			/// </summary>
			public StringBuilder sb = new StringBuilder();

			/// <summary>
			/// Clears sym and sb.
			/// </summary>
			public void Clear()
			{
				sym.Clear();
				sb.Clear();
			}
		}

		[DebuggerStepThrough]
		class _Symbol
		{
			/// C# code.
			/// <summary>
			/// If _Keyword or _Typedef - null.
			/// If _CppType - C# typename.
			/// If _Enum - "public enum...{...}".
			/// If _Struct - "public struct...{...}".
			/// If _Typedef - null.
			/// If _TypedefFunc - "public delegate...(...)".
			/// </summary>
			public string cs;
			public bool forwardDecl;

			public _Symbol(bool forwardDecl = false)
			{
				this.forwardDecl = forwardDecl;
			}
		}

		[DebuggerStepThrough]
		class _Keyword :_Symbol
		{
			public _KeywordT kwType;
			public bool cannotStartStatement;

			public _Keyword(_KeywordT kwType = _KeywordT.Normal, bool cannotStartStatement = false)
			{
				this.kwType = kwType;
				this.cannotStartStatement = cannotStartStatement;
			}
		}

		[DebuggerStepThrough]
		class _CppType :_Symbol
		{
			public byte sizeBytesCpp;
			public bool isUnsigned;
			public _CppType(string csType, int sizeBytesCpp, bool isUnsigned)
			{
				this.cs = csType;
				this.sizeBytesCpp = (byte)sizeBytesCpp;
				this.isUnsigned = isUnsigned;
            }
		}

		[DebuggerStepThrough]
		class _Enum :_Symbol
		{

			public _Enum(bool forwardDecl) : base(forwardDecl) { }
		}

		[DebuggerStepThrough]
		class _Struct :_Symbol
		{
			public _Struct(bool forwardDecl) : base(forwardDecl) { }
		}

		[DebuggerStepThrough]
		class _Typedef :_Symbol
		{
			/// <summary>
			/// pointer (1 if *, 2 if ** and so on)
			/// </summary>
			public byte ptr;
			/// <summary>
			/// TODO
			/// </summary>
			public bool isConst;
			/// <summary>
			/// struct etc for which this typedef is alias.
			/// </summary>
			public _Symbol aliasOf;

			public _Typedef(_Symbol aliasOf, int ptr, bool isConst)
			{
				this.aliasOf = aliasOf;
				this.ptr = (byte)ptr;
				this.isConst = isConst;
			}
		}

		[DebuggerStepThrough]
		class _TypedefFunc :_Symbol
		{

			public _TypedefFunc() { }
		}

		//class _Func
		//{

		//}

		[DebuggerStepThrough]
		struct _Token :IEquatable<_Token>
		{
			public char* s { get; private set; }
			public int len { get; private set; }

			public _Token(char* S, int length)
			{
				s = S;
				len = length;
			}

			public bool Equals(_Token t)
			{
				if(t.len != len) return false;
				for(int i = 0; i < len; i++) if(t.s[i] != s[i]) return false;
				return true;
			}

			public bool Equals(string s)
			{
				if(s.Length != len) return false;
				for(int i = 0; i < len; i++) if(s[i] != this.s[i]) return false;
				return true;
			}

			public override int GetHashCode()
			{
				return (int)_HashFnv1(s, len);
			}

			public override string ToString()
			{
				return new string(s, 0, len);
			}

			public static implicit operator char* (_Token t) { return t.s; }

			//FNV-1 hash.
			//2 times faster than Crc, but less accurate.
			//If length<0, data must be \0-terminated string. Then same speed (does not call Len).
			static uint _HashFnv1(char* data, int length = -1)
			{
				uint hash = 2166136261;

				if(length >= 0) {
					for(int i = 0; i < length; i++)
						hash = (hash * 16777619) ^ data[i];
					//hash = (hash ^ p[i]) * 16777619; //FNV-1a, slower
				} else {
					char ch;
					while((ch = *data++) != '\0') hash = (hash * 16777619) ^ ch;
				}

				return hash;
			}
		}
	}
}
