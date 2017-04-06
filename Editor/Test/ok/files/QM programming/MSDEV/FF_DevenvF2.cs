 \
function# iid FILTER&f

if(iid)
	if(!wintest(f.hwnd2 "" "RichEdit20W")) ret
	str s.getwintext(f.hwnd2)
	if(!s.len) ret
	mac iid s
	ret -1

'TAxf
int w1=wait(5 win("Find Symbol" "#32770"))
s=_command
s.setsel
