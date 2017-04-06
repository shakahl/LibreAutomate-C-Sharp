function# submatch str&text

 Gets offset and text of the match or a submatch found by Match().
 Returns offset in the string. Returns -1 if the submatch does not exist in the string (it is possible eg when the subexpression is like "(subexpression)?").

 submatch - 1-based submatch index, or 0 for entire match.
 text - variable that receives text.

 REMARKS
 Gets text from the string passed to Match, therefore the string must not be freed or modified.


opt noerrorshere
int k R=Get(submatch k)
if(R>=0) text.get(_m.__s R k); else text.all
ret R
