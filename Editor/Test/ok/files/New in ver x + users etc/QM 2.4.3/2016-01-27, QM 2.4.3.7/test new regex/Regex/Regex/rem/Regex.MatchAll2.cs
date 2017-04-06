function# $s ARRAY(POINT)&a [submatch] [options] [from] [sLength] ;;options: PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK|PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT

 Finds all substrings that match regular expression compiled with Compile().
 Returns the number of found substrings (a.len). Returns 0 if not found.

 s - string to search in.
 a - array variable that receives start and end offsets of matches and/or submatches. x is start offset, y is end offset.
 submatch - if 0 or omitted, creates 1-dim array containing only the matches (does not get submatches). If > 0, it is submatch index; creates 1-dim array containing only these submatches. If -1, creates 2-dim array containing matches and submatches.
 options - flags to pass to pcre2_match(). See above. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Reference</link>.
 from - offset in s where to start searching.
 sLength - s length. Can be omitted or -1 if s iz zero-terminated.

 REMARKS
 At first call Compile().
 This function calls pcre2_match() API.


opt noerrorshere ;;eg stack overflow
a=0
if(!_p) end ERR_INIT
if(!s) s=""
if(getopt(nargs)<6 or sLength<0) sLength=len(s)
int o=options&PCRE2_MATCH_FLAGS; if(o!options) end "unknown options used" 8

if !_m
	_m=pcre2_match_data_create_from_pattern(_p 0)
	_v=pcre2_get_ovector_pointer(_m)

_vCount=0
int x y ;;array dimensions
int needAll=submatch<0
str t.flags=3
rep
	if(from>=sLength) break
	
	int R=pcre2_match(_p s sLength from o _m _mc)
	if R<=0
		if(R=-1) break
		if(R=-2) _vCount=1 ;;PCRE2_ERROR_PARTIAL
		else _Error(R)
	else if(!_vCount) _vCount=pcre2_get_ovector_count(_m)
	
	if !x
		if(needAll) x=_vCount; submatch=0
		else x=1; if(submatch>=_vCount) _Error(PCRE2_ERROR_NOSUBSTRING)
		t.all(x*80)
	
	int* v=_v
	if(submatch) v+(submatch*8)
	else if(v[1]<v[0]) v[0]=pcre2_get_startchar(_m); v[1]=v[0] ;;\K in positive assertion
	
	t.geta(+v 0 x*8)
	
	y+1
	if(R=-2) break
	from=_v[1]; if(from=_v[0]) from+1

if y
	if(needAll) a.create(x y); else a.create(y)
	memcpy a.psa.pvData t t.len

ret y
