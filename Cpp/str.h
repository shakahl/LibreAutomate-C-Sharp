#pragma once
#include "stdafx.h"

#define CMP2(s, c)	((s)[0]==c[0] && (s)[1]==c[1])
#define CMP3(s, c)	(CMP2(s, c) && (s)[2]==c[2])
#define CMP4(s, c)	(CMP3(s, c) && (s)[3]==c[3])
#define CMP5(s, c)	(CMP4(s, c) && (s)[4]==c[4])
#define CMP6(s, c)	(CMP5(s, c) && (s)[5]==c[5])

//Fast memory buffer.
//Depending on required size, uses memory in fixed-size memory in this variable (eg on stack) or allocates from heap.
//Does not call ctor/dtor.
template <class T, size_t nElemOnStack = 1000>
class Buffer
{
	static const size_t c_nStackBytes = nElemOnStack * sizeof(T);

	T* _p;
	BYTE _onStack[c_nStackBytes];
public:
	explicit Buffer() noexcept { _p = (T*)_onStack; } //later call Init, unless need <= elements than nElemOnStack

	__forceinline explicit Buffer(size_t nElem) noexcept { _Init(nElem); }

	~Buffer() { FreeHeapMemory(); }

	//Frees old memory and allocates new. Does not preserve.
	T* Alloc(size_t nElem) { FreeHeapMemory(); return _Init(nElem); }

	//Frees old memory, allocates new and calls memset(0). Does not preserve.
	T* AllocAndZero(size_t nElem) { FreeHeapMemory(); return _InitZ(nElem); }

	//Reallocates memory in the grow direction, preserving existing data.
	T* Realloc(size_t nElem) { if(nElem > nElemOnStack) _Realloc(nElem * sizeof(T)); return _p; }

	//Frees heap memory. Called by dtor.
	__declspec(noinline)
		void FreeHeapMemory()
	{
		if((LPBYTE)_p != _onStack) {
			free(_p);
			_p = (T*)_onStack;
		}
	}

	operator T*() { return _p; }

	size_t Capacity() { return (LPBYTE)_p == _onStack ? nElemOnStack : _msize(_p) * sizeof(T); }

private:
	__forceinline T* _Init(size_t nElem) { return (T*)_Init(nElem * sizeof(T), c_nStackBytes); }

	T* _InitZ(size_t nElem) { return (T*)memset(_Init(nElem), 0, nElem * sizeof(T)); }

	__declspec(noinline)
		void* _Init(size_t requiredSize, size_t stackSize)
	{
		_p = (T*)_onStack;
		if(requiredSize > stackSize) _p = (T*)malloc(requiredSize);
		return _p;
	}

	__declspec(noinline)
		void _Realloc(size_t requiredSize)
	{
		if((LPBYTE)_p != _onStack) {
			_p = (T*)realloc(_p, requiredSize);
		} else {
			_p = (T*)malloc(requiredSize);
			memcpy(_p, _onStack, c_nStackBytes);
		}
	}

	//tested: the __declspec(noinline) functions are added once for all T, making template instance code very small.
};

namespace str
{
//null-safe wcslen.
inline size_t Len(STR s) { return s ? wcslen(s) : 0; }

inline bool IsEmpty(STR s) { return s == null || *s == 0; }

//Formats string by appending strings and numbers to an internal buffer (Buffer<WCHAR, 1000>.
//Can be used instead of std::wstringstream which adds ~130 KB to the dll size. I don't trust CString etc too, although did not test.
class StringBuilder
{
	static const size_t c_bufferSize = 1000;
	size_t _len, _all;
	Buffer<WCHAR, c_bufferSize> _b;

	void _ReallocIfNeed(size_t lenAppend)
	{
		auto n = _len + lenAppend;
		if(n >= _all) {
			n += _all;
			_b.Realloc(n);
			_all = n;
		}
	}
public:
	StringBuilder()
	{
		_len = 0;
		_all = c_bufferSize;
		_b[0] = 0;
	}

	void Clear()
	{
		_b.FreeHeapMemory();
		_len = 0;
		_all = c_bufferSize;
		_b[0] = 0;
	}

	operator LPWSTR() {
		return _b;
	}

	int Length() {
		return (int)_len;
	}

	BSTR ToBSTR() {
		return SysAllocStringLen(_b, (UINT)_len);
	}

	void Append(STR s, size_t lenS)
	{
		if(lenS > 0) {
			_ReallocIfNeed(lenS);
			memcpy(_b + _len, s, lenS * 2);
			auto n = _len + lenS;
			_b[n] = 0;
			_len = n;
		}
	}

	void Append(STR s) { Append(s, Len(s)); }

	friend StringBuilder& operator<<(StringBuilder& b, STR s) {
		b.Append(s);
		return b;
	}

	void AppendBSTR(BSTR s) { Append(s, SysStringLen(s)); }
	//note: cannot add Append and << overloads for BSTR because then compiler chooses them for LPWSTR etc.

	void Append(__int64 i, int radix = 10)
	{
		_ReallocIfNeed(20);
		_i64tow(i, _b + _len, radix);
		auto n = _len; while(_b[n]) n++;
		_b[n] = 0;
		_len = n;
	}

	friend StringBuilder& operator<<(StringBuilder& b, __int64 i) {
		b.Append(i);
		return b;
	}

	friend StringBuilder& operator<<(StringBuilder& b, int i) {
		b.Append((__int64)i);
		return b;
	}

	void AppendChar(WCHAR c, int count = 1)
	{
		if(count > 0) {
			_ReallocIfNeed(count);
			LPWSTR t = _b + _len;
			while(--count >= 0) *t++ = c;
			*t = 0;
			_len = t - _b;
		}
	}

	friend StringBuilder& operator<<(StringBuilder& b, WCHAR c) {
		b.AppendChar(c);
		return b;
	}

	friend StringBuilder& operator<<(StringBuilder& b, char c) {
		b.AppendChar((WCHAR)c);
		return b;
	}

	//Gets buffer that can be passed to an API function that needs it.
	//The buffer is after the formatted string, so the API will append text, not replace.
	//After calling the API, call FixBuffer.
	//size - receives available size, which is >=minSize.
	//minSize - minimal buffer size you need. Default 500.
	LPWSTR GetBufferToAppend(out int& size, int minSize = 500)
	{
		_ReallocIfNeed(minSize + 1);
		size = (int)(_all - _len - 1);
		return _b + _len;
	}

	//Sets correct length after calling GetBufferToAppend and an API function that writes to the buffer.
	//If appendLen<0, calls wcslen.
	void FixBuffer(int appendLen = -1)
	{
		auto n = _len + (appendLen<0 ? wcslen(_b + _len) : appendLen);
		assert(n < _all); if(n >= _all) n = _len;
		_b[n] = 0;
		_len = n;
	}
};

//PCRE regular expression.
//More info in the C# version.
class Regex
{
	pcre2_code_16* _code;
	pcre2_match_data_16* _md;
public:

	Regex() { _code = null; _md = null; }

	~Regex()
	{
		if(_code) {
			pcre2_code_free_16(_code); _code = null;
			if(_md != null) pcre2_match_data_free_16(_md);
		}
	}

	bool Parse(STR rx, size_t len, __int64 flags = PCRE2_UTF, out BSTR* errStr = null)
	{
		assert(_code == null);
		int errCode; size_t errOffset;
		UINT f = (UINT)flags, fe = flags >> 32;
		pcre2_compile_context_16* cc = null;
		if(fe) {
			cc = pcre2_compile_context_create_16(null);
			pcre2_set_compile_extra_options_16(cc, fe);
		}
		auto re = pcre2_compile_16(rx, len, f, &errCode, &errOffset, cc);
		if(cc) pcre2_compile_context_free_16(cc);
		if(re == null) {
			if(errStr) *errStr = GetErrorMessage(errCode, errOffset);
			return false;
		}
		_code = re;
		return true;
	}

	bool Match(STR s, size_t len, size_t start = 0, UINT flags = 0)
	{
		if(_md == null) _md = pcre2_match_data_create_from_pattern(_code, null);
		return pcre2_match(_code, s, len, start, flags, _md, null) > 0;
	}

	int GetCodeSize()
	{
		size_t codeSize = 0;
		if(_code == null || 0 != pcre2_pattern_info(_code, PCRE2_INFO_SIZE, &codeSize)) return 0;
		return (int)codeSize;
	}

	BSTR GetErrorMessage(int code, size_t offset = -1)
	{
		WCHAR b[300]; b[0] = 0;
		auto t = b;
		if(offset >= 0) {
			wcscat(t, L"Regular expression error at offset "); t += wcslen(t);
			_itow((int)offset, t, 10); t += wcslen(t);
			wcscat(t, L": "); t += 2;
		}
		pcre2_get_error_message(code, t, _countof(b) - (t - b));
		return SysAllocString(b);

		//here can instead use StringBuilder, which was created later
	}
};

//Wildcard expression.
//More info in the C# version and help file.
class Wildex
{
	/// <summary>
	/// The type of text (wildcard expression) used when creating the Wildex variable.
	/// </summary>
public:
	enum class WildType :byte
	{
		/// Simple text (option t, or no *? characters and no t r p options).
		Text,

		/// Wildcard (has *? characters and no t r p options).
		/// Match() calls str::Like.
		Wildcard,

		/// PCRE regular expression (option p or r).
		RegexPcre,

		/// Multiple parts (option m).
		/// Match() calls Match() for each part and returns true if all negative (option n) parts return true (or there are no such parts) and some positive (no option n) part returns true (or there are no such parts).
		Multi,
	};

private:
	union {
		LPWSTR _text;
		Wildex* _multi_array;
	};
	union {
		size_t _text_length;
		size_t _multi_count;
	};
	static_assert(sizeof(LPWSTR) + sizeof(size_t) == sizeof(Regex));
	WildType _type;
	bool _ignoreCase;
	bool _not;
	bool _freeText;

public:
	Wildex() { ZEROTHIS; }
	~Wildex();
	bool Wildex::Parse(STR w, size_t lenW, bool doNotCopyString = false, out BSTR* errStr = null);
	bool Match(STR s, size_t lenS) const;
	bool Is() const { return _text != null; }

	static bool HasWildcards(STR s, size_t lenS);
};

EXPORT STR Cpp_LowercaseTable();
bool Like(STR s, size_t lenS, STR w, size_t lenW, bool ignoreCase = false);
bool Equals(STR w, size_t lenW, STR s, size_t lenS, bool ignoreCase = false);

int Switch(STR s, size_t lenS, std::initializer_list<STR> a);
int Switch(STR s, std::initializer_list<STR> a);


} //namespace str


class Bstr : public CComBSTR
{
public:
	using CComBSTR::CComBSTR; //inherit ctors

	//Calls Attach(SysAllocStringLen(s, len)) and returns m_str.
	BSTR Assign(STR s, int len)
	{
		Attach(SysAllocStringLen(s, len));
		return m_str;
	}

	bool Equals(STR s, int lenS, bool ignoreCase)
	{
		if(m_str == null) return s == null;
		return str::Equals(m_str, Length(), s, lenS);
	}

	bool Equals(STR s, bool ignoreCase)
	{
		if(m_str == null) return s == null;
		return str::Equals(m_str, Length(), s, str::Len(s));
	}

	bool Equals(BSTR s, bool ignoreCase)
	{
		if(m_str == null) return s == null;
		return str::Equals(m_str, Length(), s, SysStringLen(s));
	}
};

struct BstrNameValue
{
	Bstr name;
	Bstr value;
};
