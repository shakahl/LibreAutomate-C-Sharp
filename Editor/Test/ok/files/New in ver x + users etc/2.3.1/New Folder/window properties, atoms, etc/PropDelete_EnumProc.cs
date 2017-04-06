 /
function# hwnd $lpszString hData x

if(lpszString<=0xffff) ret 1 ;;atom
 out lpszString
RemoveProp hwnd lpszString
ret 1
