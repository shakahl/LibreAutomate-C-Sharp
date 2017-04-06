function# [submatch] [int&length]

 Gets offset and length of the match or a submatch found by Match().
 Returns offset in the string. Returns -1 if the submatch does not exist in the string (it is possible eg when the subexpression is like "(subexpression)?").

 submatch - 1-based submatch index, or 0 for entire match.
 length - variable that receives length.


if(!_vCount) end ERR_INIT 2
if(submatch>=_vCount) _Error(PCRE2_ERROR_NOSUBSTRING)
if(submatch<0) end ERR_BADARG 2
int R=_v[submatch*2]
if(&length) length=_v[submatch*2+1]-R
ret R
