function# $s [ARRAY(POINT)&a] [submatch] [options] [iFrom] [iTo] ;;options: PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK

 Finds all substrings that match regular expression compiled with Compile().
 Returns the number of found substrings (a.len). Returns 0 if not found.

 s - string to search in.
 a - array variable that receives start and end offsets of matches and/or submatches. x is start offset, y is end offset. Can be omitted or 0 if you don't need it.
 submatch - if 0 or omitted, creates 1-dim array containing only the matches (does not get submatches). If > 0, it is submatch index; creates 1-dim array containing only these submatches. If -1, creates 2-dim array containing matches and submatches.
 options - flags to pass to pcre2_match(). See above. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Reference</link>.
 iFrom - offset in s where to begin searching.
 iTo - s length or offset in s where to end searching. If omitted or -1, calculates s length like <help>len</help>.

 REMARKS
 At first call Compile().
 This function calls pcre2_match() API.


opt noerrorshere ;;eg stack overflow
if(&a) a=0
if(!_p) end ERR_INIT
if((options&(PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT))) _Error(PCRE2_ERROR_BADOPTION)
if(!s) s=""
if(getopt(nargs)<6 or iTo<0) iTo=len(s)

if(!_m) _m=pcre2_match_data_create_from_pattern(_p 0); _v=pcre2_get_ovector_pointer(_m)

_vCount=0
int x y ;;array dimensions
int r needAll=submatch<0
str t
int* v
rep
	r=pcre2_match(_p s iTo iFrom options _m _mc)
	
	if(r<=0) if(r=-1) break; else _vCount=0; _Error(r)
	
	if(_v[0]>_v[1]) _v[0]=pcre2_get_startchar(_m); _v[1]=_v[0] ;;\K in positive assertion
	
	if !y ;;init variables
		_vCount=pcre2_get_ovector_count(_m)
		
		if(needAll) x=_vCount; v=_v
		else x=1; v=_v+(submatch*8); if(submatch>=_vCount) _Error(PCRE2_ERROR_NOSUBSTRING)
		
		if(&a) t.all(x*80); t.flags=3
	
	if(&a) t.geta(+v 0 x*8)
	
	y+1
	iFrom=_v[1]; if(iFrom=_v[0]) iFrom+1
	if(iFrom>iTo) break

if y and &a
	if(needAll) a.create(x y); else a.create(y)
	memcpy a.psa.pvData t t.len

ret y
