function# $s [options] [from] [sLength] ;;options: PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK|PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT

 Finds first substring that matches regular expression compiled by Compile().
 Returns 1 + the number of submatches. Returns 0 if not found. Returns -2 if partial match found (flags PCRE2_PARTIAL_X).

 s - string to search in.
 options - flags to pass to pcre2_match(). See above. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Reference</link>.
 from - offset in s where to start searching.
 sLength - s length. Can be omitted or -1 if s iz zero-terminated.

 REMARKS
 At first call Compile(). Then call Match() one or more times. If Match() returns not 0, you can call Get[ByName][Str]() functions to get results.
 This function calls pcre2_match() API.


opt noerrorshere ;;eg stack overflow
if(!_p) end ERR_INIT
if(!s) s=""
if(getopt(nargs)<4) sLength=-1
int o=options&PCRE2_MATCH_FLAGS; if(o!options) end "unknown options used" 8

if !_m
	_i=0

_vCount=0
 int R=pcre2_match(_p s sLength from o _m _mc)
 
 if R>0
	 R=pcre2_get_ovector_count(_m)
	 _vCount=R
 else
	 if(R=-1) ret
	 if(R=-2) _vCount=1 ;;PCRE2_ERROR_PARTIAL
	 else _Error(R)
 
 if(_v[0]>_v[1]) _v[0]=pcre2_get_startchar(_m); _v[1]=_v[0] ;;\K in positive assertion
 
 ret R
