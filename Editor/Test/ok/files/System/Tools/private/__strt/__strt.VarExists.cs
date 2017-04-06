function! [$vType]

 Returns 1 if a variable of name=this of type=vType exists in current macro.
 If vType not used, returns 1 if a variable of any type or an intrinsic function with the name exists.
 Returns 0 if not valid name.
 Currently ignores #sub.

if(!IsValidName) ret
int R=__LocalVarUniqueName(s &_s 0 vType)
if R
	if(empty(vType)) ret 1
	ret R=-1
