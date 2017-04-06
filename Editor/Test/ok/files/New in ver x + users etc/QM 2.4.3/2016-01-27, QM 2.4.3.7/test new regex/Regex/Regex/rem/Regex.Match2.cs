function# $s [int&matchLength] [submatch] [options] [from] [sLength] ;;options: PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK|PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT


opt noerrorshere ;;eg can be stack overflow
if(!_p) end ERR_INIT
if(getopt(nargs)<6) sLength=-1
int o=options&PCRE2_MATCH_FLAGS; if(o!options) end "unknown options used" 8
if(!_m) _m=pcre2_match_data_create_from_pattern(_p 0)
