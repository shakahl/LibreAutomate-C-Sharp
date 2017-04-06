function# $subName str&text

 Gets offset and text of a submatch found by Match().
 Returns offset in the string. Returns -1 if the submatch does not exist in the string (it is possible eg when the subexpression is like "(subexpression)?").

 subName - subexpression name. In regular expression can be specified like "(?<name>...)".
 text - variable that receives text.

 REMARKS
 Gets text from the string passed to Match, therefore the string must not be freed or modified.


opt noerrorshere
int k R=GetByName(subName k)
if(R>=0) text.get(_m.__s R k); else text.all
ret R
