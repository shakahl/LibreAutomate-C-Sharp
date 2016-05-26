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

		enum _SymT :byte
		{
			Keyword, //C++ keyword except types
			CppType, //C++ intrinsic type
			Enum,
			Struct,
			Typedef,
			TypedefFunc,
			//AnyType //any except Keyword (not used in _Symbol class)
		}

		enum _KeywordT :byte
		{
			Normal,
			TypeDecl, //enum, struct, union, class, __interface, typedef
			CallConv, //__stdcall etc
			Inline, //__forceinline etc
			PubPrivProt, //public, private, protected
			Ignore, //volatile etc
		}

		class _Symbol
		{
			internal _SymT symType;
			internal bool forwardDecl;

			internal _Symbol(_SymT symType, bool forwardDecl = false)
			{
				this.symType = symType;
				this.forwardDecl = forwardDecl;
			}
		}

		class _Keyword :_Symbol
		{
			internal _KeywordT kwType;
			internal bool cannotStartStatement;

			internal _Keyword(_KeywordT kwType = _KeywordT.Normal, bool cannotStartStatement = false) : base(_SymT.Keyword)
			{
				this.kwType = kwType;
				this.cannotStartStatement = cannotStartStatement;
			}
		}

		class _CppType :_Symbol
		{
			public string csType;

			internal _CppType(string csType) : base(_SymT.CppType) { this.csType = csType; }
		}

		class _Enum :_Symbol
		{

			internal _Enum(bool forwardDecl) : base(_SymT.Enum, forwardDecl) { }
		}

		class _Struct :_Symbol
		{
			public int membersOffset, membersLength;

			internal _Struct(bool forwardDecl) : base(_SymT.Struct, forwardDecl) { }
		}

		class _Typedef :_Symbol
		{
			/// <summary>
			/// pointer (1 if *, 2 if ** and so on)
			/// </summary>
			internal byte ptr;
			/// <summary>
			/// TODO
			/// </summary>
			internal bool isConst;
			/// <summary>
			/// when eg 'typedef struct tagX{...} X;
			/// </summary>
			internal bool ofTag;
			/// <summary>
			/// struct etc for which this typedef is alias.
			/// </summary>
			internal _Symbol aliasOf;

			internal _Typedef(_Symbol aliasOf, int ptr, bool ofTag) : base(_SymT.Typedef)
			{
				this.aliasOf = aliasOf;
				this.ptr = (byte)ptr;
				this.ofTag = ofTag;
			}
		}

		class _TypedefFunc :_Symbol
		{

			internal _TypedefFunc() : base(_SymT.TypedefFunc) { }
		}

		//class _Func
		//{

		//}

		struct _Token :IEquatable<_Token>
		{
			public char* s { get; private set; }
			public int len { get; private set; }

			internal _Token(char* S, int length)
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
