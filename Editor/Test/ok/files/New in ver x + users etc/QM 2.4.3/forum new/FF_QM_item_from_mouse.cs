 /
function# iid FILTER&f

if f.ttype!=2
	f.hwnd2=child(mouse)
	f.hwnd=GetAncestor(f.hwnd2 2)
if(!f.hwnd2) ret -2
if(f.hwnd!=_hwndqm) ret -2
if(GetWinId(f.hwnd2)=2202) ret iid
ret -2
