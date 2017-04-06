function# $subName [int&length]

 Gets offset and length of a submatch found by Match().
 Returns offset in the string. Returns -1 if the submatch does not exist in the string (it is possible eg when the subexpression is like "(subexpression)?").

 subName - subexpression name. In regular expression can be specified like "(?<name>...)".
 length - variable that receives length.


opt noerrorshere
if(!_vCount) end ERR_INIT
int n=_SubNameToNumber(subName)
if(n<0) _Error(n)
if(n>=_vCount) _Error(PCRE2_ERROR_NOSUBSTRING) ;;if partial match
ret Get(n length)
