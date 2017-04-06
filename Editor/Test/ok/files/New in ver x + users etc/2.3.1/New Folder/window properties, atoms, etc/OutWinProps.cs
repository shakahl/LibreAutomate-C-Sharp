 /
function hwnd
EnumPropsEx hwnd &sub.OWP_Proc 0


#sub OWP_Proc
function# hwnd $lpszString hData x

if(lpszString<=0xffff)
	_s.fix(GlobalGetAtomName(+lpszString _s.all(300) 300))
	_s+" (atom)"
	lpszString=_s
out "%s   %i" lpszString hData
ret 1
