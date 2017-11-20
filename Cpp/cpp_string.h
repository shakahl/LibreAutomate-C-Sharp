#pragma once

class Cpp_Regex
{
	pcre2_code_16* _code;
	pcre2_match_data_16* _md;
public:

	Cpp_Regex() { _code = null; _md = null; }

	~Cpp_Regex()
	{
		if(_code) {
			pcre2_code_free_16(_code); _code = null;
			if(_md != null) pcre2_match_data_free_16(_md);
		}
	}

	bool Parse(STR rx, size_t len, __int64 flags = PCRE2_UTF, out LPWSTR* errStr = null)
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

	LPWSTR GetErrorMessage(int code, size_t offset = -1)
	{
		WCHAR b[300]; b[0] = 0;
		auto t = b;
		if(offset >= 0) {
			wcscat(t, L"Regular expression error at offset "); t += wcslen(t);
			_itow((int)offset, t, 10); t += wcslen(t);
			wcscat(t, L": "); t += 2;
		}
		pcre2_get_error_message(code, t, _countof(b) - (t - b));
		return _wcsdup(b);
	}
};

class CppWildex
{
	/// <summary>
	/// The type of text (wildcard expression) used when creating the Wildex variable.
	/// </summary>
public:
	enum class WildType :byte
	{
		/// <summary>
		/// Simple text (option t, or no *? characters and no t r options).
		/// </summary>
		Text,

		/// <summary>
		/// Wildcard (has *? characters and no t r options).
		/// Match() calls <see cref="Cpp_StringLike"/>.
		/// </summary>
		Wildcard,

		/// <summary>
		/// PCRE regular expression (option p or r).
		/// </summary>
		RegexPcre,

		/// <summary>
		/// Multiple parts (option m).
		/// Match() calls Match() for each part (see <see cref="MultiArray"/>) and returns true if all negative (option n) parts return true (or there are no such parts) and some positive (no option n) part returns true (or there are no such parts).
		/// If you want to implement a different logic, call Match() for each <see cref="MultiArray"/> element (instead of calling Match() for this variable).
		/// </summary>
		Multi,
	};

private:
	union {
		LPWSTR _text;
		CppWildex* _multi_array;
	};
	union {
		size_t _text_length;
		size_t _multi_count;
	};
	static_assert(sizeof(LPWSTR) + sizeof(size_t) == sizeof(Cpp_Regex));
	WildType _type;
	bool _ignoreCase;
	bool _not;
	bool _freeText;

public:
	CppWildex() { ZEROTHIS; }
	~CppWildex();
	bool CppWildex::Parse(STR w, size_t lenW, bool doNotCopyString = false, out LPWSTR* errStr = null);
	bool Match(STR s, size_t lenS);
	bool Is() { return _text != null; }

	static bool HasWildcards(STR s, size_t lenS);
};


//Formats string by appending strings and numbers to an internal buffer.
//The buffer is simple C WCHAR array of size 5000. The Append functions do nothing if cannot append full string because the buffer is too small.
//Can be used instead of std::wstringstream which adds ~130 KB to the dll size. I don't trust CString too, although did not test.
class SimpleStringBuilder
{
	static const int c_bufferSize = 5000;
	int _len;
	WCHAR _b[c_bufferSize];

public:
	SimpleStringBuilder()
	{
		_len = 0;
		_b[0] = 0;
	}

	void Clear()
	{
		_len = 0;
		_b[0] = 0;
	}

	LPWSTR str() {
		return _b;
	}

	operator LPWSTR() {
		return _b;
	}

	void Append(STR s, size_t len)
	{
		if(len > 0) {
			auto n = _len + len;
			assert(n < c_bufferSize);
			if(n < c_bufferSize) {
				memcpy(_b + _len, s, len * 2);
				_b[n] = 0;
				_len = (int)n;
			}
		}
	}

	void Append(STR s)
	{
		if(s && s[0]) Append(s, wcslen(s));
	}

	friend SimpleStringBuilder& operator<<(SimpleStringBuilder& b, STR s) {
		b.Append(s);
		return b;
	}

	void Append(__int64 i, int radix = 10)
	{
		int n = _len + 20;
		assert(n < c_bufferSize);
		if(n < c_bufferSize) {
			_i64tow(i, _b + _len, radix);
			for(n = _len; _b[n]; n++) { }
			_b[n] = 0;
			_len = n;
		}
	}

	friend SimpleStringBuilder& operator<<(SimpleStringBuilder& b, __int64 i) {
		b.Append(i);
		return b;
	}

	friend SimpleStringBuilder& operator<<(SimpleStringBuilder& b, int i) {
		b.Append((__int64)i);
		return b;
	}

	void AppendChar(WCHAR c, int count = 1)
	{
		if(count > 0) {
			auto n = _len + count;
			assert(n < c_bufferSize);
			if(n < c_bufferSize) {
				LPWSTR t = _b + _len;
				while(--count >= 0) *t++ = c;
				*t = 0;
				_len = (int)(t - _b);
			}
		}
	}

	friend SimpleStringBuilder& operator<<(SimpleStringBuilder& b, WCHAR c) {
		b.AppendChar(c);
		return b;
	}

	LPWSTR GetBufferToAppend(out int& size)
	{
		size = c_bufferSize - _len - 1;
		return _b + _len;
	}

	void FixBuffer(int appendLen = -1)
	{
		if(appendLen < 0) appendLen = (int)wcslen(_b + _len);
		auto n = _len + appendLen;
		assert(n < c_bufferSize);
		if(n >= c_bufferSize) n = _len;
		_b[n] = 0;
		_len = (int)n;
	}
};
