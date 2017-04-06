 /
function hwnd ARRAY(STRINT)&a

 Gets all window property names and values.


a=0
EnumPropsEx hwnd &sub.EnumProc &a


#sub EnumProc
function# hwnd $lpszString hData ARRAY(STRINT)&a

if(lpszString<=0xffff) lpszString=_s.format("#%i" lpszString)
a[].s=lpszString
a[a.ubound].i=hData
ret 1
