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


opt noerrorshere
a=0
if(!_p) end ERR_INIT
if(getopt(nargs)<6 or sLength<0) sLength=len(s)
int o=options&PCRE2_MATCH_FLAGS; if(o!options) end "unknown options used" 8

int x y ;;array dimensions
int r needAll=submatch<0
str t
int* v
rep
	if(from>=sLength) break
	
	r=Match(s o from sLength)
	if(!r) break
	
	_i=Match4(s o from sLength)
	
	if !y
		if(needAll) x=_vCount; v=_v
		else x=1; v=_v+(submatch*8); if(submatch>=_vCount) _Error(PCRE2_ERROR_NOSUBSTRING)
		t.all(x*80); t.flags=3
	y+1
	
	t.geta(+v 0 x*8)
	
	if(r<0) break ;;partial match
	from=_v[1]; if(from=_v[0]) from+1

if y
	if(needAll) a.create(x y); else a.create(y)
	memcpy a.psa.pvData t t.len

ret y
