function# str&s [$repl] [options] [iFrom] [iTo] ;;options: PCRE2_SUBSTITUTE_GLOBAL|PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK

 Finds and replaces substring(s) that match regular expression compiled with Compile().
 Returns the number of replacements. Returns 0 if not found.

 s - string to search/replace in.
 repl - replacement string. Can contain special substrings like with <help>str.replacerx</help>. For example, $0 or ${0} is entire match, $1 or ${0} is the first submatch, ${name} is a named submatch.
 options - flags to pass to pcre2_substitute(). See above. Use flag PCRE2_SUBSTITUTE_GLOBAL (0x100) to replace all. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Other flags</link> are like with Match().
 iFrom - offset in s where to begin searching/replacing.
 iTo - offset in s where to end searching/replacing. If omitted or -1, uses s.len.

 REMARKS
 At first call Compile().
 This function calls pcre2_substitute() API. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC34">Reference</link>.


opt noerrorshere ;;eg stack overflow
if(!_p) end ERR_INIT
if(!s) s=""
if(getopt(nargs)<5 or iTo<0) iTo=s.len
if(iFrom<0 or iTo>s.len) end ERR_BADARG

int rlen=len(repl)
int n=s.len; if(rlen) n+n/2+rlen+1000
 g1
str t.all(n)
int R=pcre2_substitute(_p s iTo iFrom options _m _mc repl rlen t &n)
if R<0
	if(R=PCRE2_ERROR_NOMEMORY) n*2; goto g1
	_Error(R)

if R
	if(iTo=s.len) s.left(t.lpstr n)
	else s.fromn(t.lpstr n s+iTo s.len-iTo)

ret R
