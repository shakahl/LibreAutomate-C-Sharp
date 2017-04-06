function# $s [options] [iFrom] [iTo] ;;options: PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK|PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT

 Finds first substring that matches regular expression compiled with Compile().
 Returns 1 + the number of submatches. Returns 0 if not found. Returns -2 if partial match found (flags PCRE2_PARTIAL_X).

 s - string to search in.
 options - flags to pass to pcre2_match(). See above. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC26">Reference</link>.
 iFrom - offset in s where to begin searching.
 iTo - s length or offset in s where to end searching. If omitted or -1, calculates s length like <help>len</help>.

 REMARKS
 At first call Compile(). Then call Match() one or more times. If Match() returns not 0, you can call Get[ByName][Str]() functions to get results.
 This function calls pcre2_match() API.


if(getopt(nargs)<4) iTo=-1

int R=pcre2qm_match(&this s options iFrom iTo)
if(R<-2) end F"{R} {pcre2qm_last_error}"
ret R
