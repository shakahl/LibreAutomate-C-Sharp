#include "stdafx.h"
#include "cpp.h"

static WCHAR _caseTable[0x10000];
static bool _caseTableCreated;

//Returns static WCHAR table[0x10000] containing all Unicode characters in that range. Uppercase characters converted to lowercase.
extern "C" __declspec(dllexport)
STR Cpp_LowercaseTable()
{
	if(!_caseTableCreated) {
		LPWSTR t = _caseTable;
		for(size_t i = 0; i < 0x10000; i++) t[i] = (WCHAR)i;
		CharLowerBuff(t, 0x10000);
		_caseTableCreated = true;
	} //speed: 350
	return _caseTable;
}

inline STR _LowercaseTable() {
	return _caseTableCreated ? _caseTable : Cpp_LowercaseTable();
}

//Compares string s of length lenS with wildcard pattern w of length lenW.
//Returns true if match. Returns false if s==null. Exception if w==null.
//extern "C" __declspec(dllexport) //C# has own function. Calling this from C# is significantly slower when string short.
bool Cpp_StringLike(STR s, size_t lenS, STR w, size_t lenW, bool ignoreCase = false)
{
	if(s == null) return false;
	if(lenW == 0) return lenS == 0;
	if(lenW == 1 && w[0] == '*') return true;
	if(lenS == 0) return false;

	STR table = ignoreCase ? _LowercaseTable() : null;
	STR se = s + lenS, we = w + lenW;

	//find '*' from start. Makes faster in some cases.
	for(; (w < we && s < se); w++, s++) {
		size_t cS = s[0], cW = w[0];
		if(cW == '*') goto g1;
		if(cW == cS || cW == '?') continue;
		if((table == null) || (table[cW] != table[cS])) return false;
	}
	if(w == we) return s == se; //p ended?
	goto gr; //s ended
g1:

	//find '*' from end. Makes "*text" much faster.
	for(; (we > w && se > s); we--, se--) {
		size_t cS = se[-1], cW = we[-1];
		if(cW == '*') break;
		if(cW == cS || cW == '?') continue;
		if((table == null) || (table[cW] != table[cS])) return false;
	}

	//Algorithm by Alessandro Felice Cantatore, http://xoomer.virgilio.it/acantato/dev/wildcard/wildmatch.html
	//Changes: supports '\0' in string; case-sensitive or not; restructured, in many cases faster.

	size_t i = 0;
gStar: //info: goto used because C# compiler makes the loop faster when it contains less code
	w += i + 1;
	if(w == we) return true;
	s += i;

	for(i = 0; s + i < se; i++) {
		size_t sW = w[i];
		if(sW == '*') goto gStar;
		if(sW == s[i] || sW == '?') continue;
		if((table != null) && (table[sW] == table[s[i]])) continue;
		s++; i = -1;
	}

	w += i;
gr:
	while(w < we && *w == '*') w++;
	return w == we;

	//info: Could implement escape sequence ** for * and maybe *? for ?.
	//	But it makes code slower etc.
	//	Not so important.
	//	Most users would not know about it.
	//	Usually can use ? for literal * and ?.
	//	Usually can use regular expression if need such precision.
	//	Could not use "**options|text" for wildcard expressions.
	//	Could use other escape sequences, eg [*], [?] and [[], but it makes slower and is more harmful than useful.

	//The first two loops are fast, but Equals_ much faster when !ignoreCase. We cannot use such optimizations that it can.
	//The slowest case is "*substring*", because then the first two loops don't help.
	//	Then similar speed as string.IndexOf(ordinal) and API <msdn>FindStringOrdinal</msdn>.
	//	Possible optimization, but need to add much code, and makes not much faster, and makes other cases slower, difficult to avoid it.
}

//Compares string s of length lenS with string w of length lenW.
//Returns true if match. Returns false if s==null. Exception if w==null.
//extern "C" __declspec(dllexport) //C# has own function. Calling this from C# is significantly slower when string short.
bool Cpp_StringEquals(STR w, size_t lenW, STR s, size_t lenS, bool ignoreCase = false)
{
	if(s == null || lenS != lenW) return false;
	if(lenW == 0) return true;

	if(!ignoreCase) return 0 == memcmp(s, w, lenW * 2); //fastest

	//optimization: at first compare case-sensitive, as much as possible.
	//	never mind: in 32-bit process this is not the fastest code (too few registers). But makes much faster anyway.
	//	tested: strings don't have to be aligned at 4 or 8.
	while(lenW >= 12) {
		if(*(__int64*)w != *(__int64*)s) break;
		if(*(__int64*)(w + 4) != *(__int64*)(s + 4)) break;
		if(*(__int64*)(w + 8) != *(__int64*)(s + 8)) break;
		w += 12; s += 12; lenW -= 12;
	}

	auto table = _LowercaseTable();
	for(size_t i = 0; i < lenW; i++) {
		size_t c1 = w[i], c2 = s[i];
		if(c1 != c2 && table[c1] != table[c2]) goto gFalse;
	}
	return true; gFalse: return false;
}

//this could be used by C#, but calling unmanaged functions from C# is slow when with 'fixed', which makes much slower with short strings.
//extern "C" __declspec(dllexport)
//bool Cpp_StringEqualsI(STR a, STR b, int len)
//{
//	if(len != 0) {
//		//optimization: at first compare case-sensitive, as much as possible.
//		//	never mind: in 32-bit process this is not the fastest code (too few registers). But makes much faster anyway.
//		//	tested: strings don't have to be aligned at 4 or 8.
//		while(len >= 12) {
//			if(*(__int64*)a != *(__int64*)b) break;
//			if(*(__int64*)(a + 4) != *(__int64*)(b + 4)) break;
//			if(*(__int64*)(a + 8) != *(__int64*)(b + 8)) break;
//			a += 12; b += 12; len -= 12;
//		}
//
//		auto table = _LowercaseTable();
//
//		for(size_t i = 0; i < len; i++) {
//			size_t c1 = a[i], c2 = b[i];
//			if(c1 != c2 && table[c1] != table[c2]) goto gFalse;
//		}
//	}
//	return true; gFalse: return false;
//}



extern "C" __declspec(dllexport)
bool Cpp_RegexParse(Cpp_Regex& x, STR rx, size_t len, __int64 flags = PCRE2_UTF, out LPWSTR* errStr = null) {
	return x.Parse(rx, len, flags, errStr);
}

extern "C" __declspec(dllexport)
bool Cpp_RegexMatch(Cpp_Regex& x, STR s, size_t len, size_t start = 0, UINT flags = 0) {
	return x.Match(s, len, start, flags);
}

extern "C" __declspec(dllexport)
void Cpp_RegexDtor(Cpp_Regex& x) {
	x.~Cpp_Regex();
}

extern "C" __declspec(dllexport)
int Cpp_RegexSize(Cpp_Regex& x) {
	return x.GetCodeSize();
}


extern "C" __declspec(dllexport)
void Cpp_Free(void* p) {
	if(p) free(p);
}


//extern "C" __declspec(dllexport)
//bool Cpp_WildexParse(CppWildex& x, STR w, size_t lenW, out LPWSTR* errStr) {
//	return x.Parse(w, lenW, false, errStr);
//}
//
//extern "C" __declspec(dllexport)
//bool Cpp_WildexMatch(CppWildex& x, STR s, size_t lenS) {
//	return x.Match(s, lenS);
//}
//
//extern "C" __declspec(dllexport)
//void Cpp_WildexDtor(CppWildex& x) {
//	x.~CppWildex();
//}

CppWildex::~CppWildex()
{
	if(_text != null) {
		switch(_type) {
		case WildType::RegexPcre:
			((Cpp_Regex*)&_text)->~Cpp_Regex();
			break;
		case WildType::Multi: {
			for(size_t i = 0; i < _multi_count; i++) _multi_array[i].~CppWildex();
			free(_multi_array);
		} break;
		default:
			if(_freeText) free(_text);
		}
		_text = null;
	}
}

//Parses wildcard string and initializes this variable.
//On error sets errStr (if not null) and returns false.
bool CppWildex::Parse(STR w, size_t lenW, bool doNotCopyString/* = false*/, out LPWSTR* errStr/* = null*/)
{
	if(errStr) *errStr = null;
	void** g = (void**)&_text;
	assert(_text == null);
	//if(w == null) return false;
	_type = WildType::Wildcard;
	_ignoreCase = true;

	if(lenW >= 3 && w[0] == '*' && w[1] == '*') {
		for(size_t i = 2; i < lenW; i++) {
			switch(w[i]) {
			case 't': _type = WildType::Text; break;
			case 'p': case 'r': _type = WildType::RegexPcre; break;
			case 'm': _type = WildType::Multi; break;
			case 'c': _ignoreCase = false; break;
			case 'n': _not = true; break;
			case '|': w += ++i; lenW -= i; goto g1;
			default: goto ge;
			}
		}
	ge:
		if(errStr) *errStr = _wcsdup(L"Invalid **options|");
		return false;
	g1:
		switch(_type) {
		case WildType::RegexPcre: {
			__int64 f = _ignoreCase ? PCRE2_CASELESS : 0;
			if(!((Cpp_Regex*)&_text)->Parse(w, lenW, f | PCRE2_UTF, errStr)) return false;
		} goto gr;
		case WildType::Multi: {
			size_t count = 1;
			auto eos = w + lenW - 1;
			for(auto t = w; t < eos; t++) if(t[0] == '[' && t[1] == ']') count++;
			auto a = (CppWildex*)calloc(count, sizeof(CppWildex));

			for(size_t i = 0; i < count; i++, w += 2) {
				STR wi = w;
				if(i == count - 1) w = eos + 1; else { for(; w < eos; w++) if(w[0] == '[' && w[1] == ']') break; }
				if(!a[i].Parse(wi, w - wi, true, errStr)) return false;
			}

			_multi_array = a;
			_multi_count = count;
		} goto gr;
		}
	}

	if(_type == WildType::Wildcard && !HasWildcards(w, lenW)) _type = WildType::Text;
	if(doNotCopyString) _text = (LPWSTR)w;
	else {
		_text = (LPWSTR)malloc((lenW + 1) * 2); memcpy(_text, w, lenW * 2); _text[lenW] = 0;
		_freeText = true;
	}
	_text_length = lenW;
gr:
	return true;
}

bool CppWildex::Match(STR s, size_t lenS)
{
	if(s == null) return false;

	bool R = false;
	switch(_type) {
	case WildType::Wildcard:
		R = Cpp_StringLike(s, lenS, _text, _text_length, _ignoreCase);
		break;
	case WildType::Text:
		R = Cpp_StringEquals(s, lenS, _text, _text_length, _ignoreCase);
		break;
	case WildType::RegexPcre:
		R = ((Cpp_Regex*)&_text)->Match(s, lenS);
		break;
	case WildType::Multi:
		//[n] parts: all must match (with their option n applied)
		int nNot = 0;
		for(size_t i = 0; i < _multi_count; i++) {
			auto v = _multi_array + i;
			if(v->_not) {
				if(!v->Match(s, lenS)) return _not; //!v->Match(s) means 'matches if without option n applied'
				nNot++;
			}
		}
		if(nNot == _multi_count) return !_not; //there are no parts without option n

		//non-[n] parts: at least one must match
		for(size_t i = 0; i < _multi_count; i++) {
			auto v = _multi_array + i;
			if(!v->_not && v->Match(s, lenS)) return !_not;
		}
		break;
	}
	return R ^ _not;
}

/// <summary>
/// Returns true if string contains wildcard characters: '*', '?'.
/// </summary>
/// <param name="s">Can be null.</param>
/*static*/ bool CppWildex::HasWildcards(STR s, size_t lenS)
{
	if(s == null) return false;
	for(size_t i = 0; i < lenS; i++) {
		auto c = s[i];
		if(c == '*' || c == '?') goto yes;
	}
	return false; yes: return true;
}
