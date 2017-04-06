
 Frees the compiled regular expression and other data and clears this variable.
 Usually don't need to call this function. It is called implicitly when the variable dies.


if(_p) pcre2_code_free(_p); _p=0
if(_m) pcre2_match_data_free(_m); _m=0
if(_mc) pcre2_match_context_free(_mc); _mc=0
_v=0
_vCount=0
