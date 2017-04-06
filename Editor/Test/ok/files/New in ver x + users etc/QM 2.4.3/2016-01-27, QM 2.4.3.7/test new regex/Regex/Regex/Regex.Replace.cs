function# str&s [$repl] [options] [iFrom] [iTo] [cbFunc] [cbParam] ;;options: PCRE2_SUBSTITUTE_GLOBAL|PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK

 Finds and replaces substring(s) that match regular expression compiled with Compile().
 Returns the number of replacements. Returns 0 if not found.

 s - string to search/replace in.
 repl - replacement string. Can contain special substrings like with <help>str.replacerx</help>. For example, $0 or ${0} is entire match, $1 or ${0} is the first submatch, ${name} is a named submatch.
 options - flags to pass to pcre2_substitute(). See above. Use flag PCRE2_SUBSTITUTE_GLOBAL (0x100) to replace all. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Other flags</link> are like with Match().
 iFrom - offset in s where to begin searching/replacing.
 iTo - offset in s where to end searching/replacing. If omitted or -1, uses s.len.
 cbFunc, cbParam - callback function address, and some value to pass to it. If used, the callback function will be called for each match to provide replacements, and the repl parameter ignored. It must be:
   function# cbParam str&match REGEX_REPLACE_CALLBACK&x
    cbParam - cbParam passed to Replace().
    match - contains the match string. The callback function can modify it. It will be the replacement.
    x - contains some info about the match:
      x.t - string that is being created from replacements and nonmatching portions of s. Finally it will replace s. Now it contains the portion of s from the beginning to the current match, with previous replacements.
      x.s - the initial string (Replace() parameter s).
      x.v - array of match/submatch offsets in s. First element (v[0]) contains offsets (beginning and end) of entire match. Other elements contain offsets for submatches; the offsets are -1 for subexpressions that matched 0 times, eg "(subexpression)?".
      x.vLen - v length (1 + the number of submatches).
      x.x - address of the Regex variable. You can call its GetX functions to get current match and submatches; it is slower than using v directly, but easier to get named submatches.
      x.number - 1-based index of current match.
      x.repl - repl passed to Replace().
    Return:
      0 - match is the replacement.
      >0 - let the replacement be none. The callback function itself may append a replacement to x.t.
      -1 - stop and don't include this replacement.
      -2 - stop and include this replacement.
      <-1 - generate error with this error number.

 REMARKS
 At first call Compile().
 This function calls pcre2_match() API.


opt noerrorshere ;;eg stack overflow
if(!_p) end ERR_INIT
if(options&PCRE2_SUBSTITUTE_GLOBAL) options~PCRE2_SUBSTITUTE_GLOBAL; int global=1
if((options&(PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT))) _Error(PCRE2_ERROR_BADOPTION)
if(!s) s=""
if(getopt(nargs)<5 or iTo<0) iTo=s.len
if(iFrom<0 or iTo>s.len) end ERR_BADARG

if(!_m) _m=pcre2_match_data_create_from_pattern(_p 0); _v=pcre2_get_ovector_pointer(_m)

_vCount=0
str t
int r y iFrom2
rep
	r=pcre2_match(_p s iTo iFrom options _m _mc)
	
	if(r<=0) if(r=-1) break; else _vCount=0; _Error(r)
	
	if(_v[0]>_v[1]) _v[0]=pcre2_get_startchar(_m); _v[1]=_v[0] ;;\K in positive assertion
	
	if !y ;;init variables
		_vCount=pcre2_get_ovector_count(_m)
		
		int replType rlen ;;replType: 0 empty, 1 simple, 2 with $, 3 callback
		if(cbFunc) replType=3; else rlen=len(repl); if(rlen) replType=1+(findc(repl '$')>=0)
		
		int na=s.len; if(replType) na+na/2+rlen+1000
		t.all(na); t.flags=3
	y+1
	
	t.geta(s iFrom2 _v[0]-iFrom2)
	
	sel replType
		case 1
		t.geta(repl 0 rlen)
		
		case 2
		
		case 3
		
	
	iFrom2=_v[1]
	if(!global) break
	iFrom=iFrom2+(iFrom2=_v[0])
	if(iFrom>iTo) break

if y
	int tail=s.len-iFrom2
	if(tail>0) t.geta(s iFrom2 tail)
	s=t

ret y
