function# $s ARRAY(str)&a [submatch] [options] [iFrom] [iTo] ;;options: PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK

 Finds all substrings that match regular expression compiled with Compile().
 Returns the number of found substrings (a.len). Returns 0 if not found.

 s - string to search in.
 a - array variable that receives text of matches and/or submatches.
 submatch - if 0 or omitted, creates 1-dim array containing only the matches (does not get submatches). If > 0, it is submatch index; creates 1-dim array containing only these submatches. If -1, creates 2-dim array containing matches and submatches.
 options - flags to pass to pcre2_match(). See above. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Reference</link>.
 iFrom - offset in s where to begin searching.
 iTo - s length or offset in s where to end searching. If omitted or -1, calculates s length like <help>len</help>.

 REMARKS
 At first call Compile().
 This function calls pcre2_match() API.
 Returns 0 if s (or its part iFrom to iTo) is empty, even if the regular expression can match empty string.


opt noerrorshere; opt nowarningshere
if(getopt(nargs)<6) iTo=-1
a=0

ARRAY(POINT) ap
int R=MatchAll(s ap submatch options iFrom iTo)
if(!R) ret

int i n=R
if(ap.ndim=1) a.create(n); else _i=ap.len(1); a.create(_i n); n*_i
POINT* p=ap.psa.pvData; str* z=a.psa.pvData
for i 0 n
	POINT& r=p[i]
	if(r.y>r.x) z[i].get(s r.x r.y-r.x)

ret R
