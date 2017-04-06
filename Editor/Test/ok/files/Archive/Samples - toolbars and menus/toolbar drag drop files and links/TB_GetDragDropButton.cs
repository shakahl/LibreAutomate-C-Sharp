 /
function# hWnd [str&tbLabel] [str&tbLine]

 Call this function on WM_QM_DRAGDROP in toolbar hook function.
 Gets some info about the button on which dropped.
 Returns toolbar line index. Returns 0 if dropped not on a button.

 hWnd - hWnd.
 tbLabel - receives button text. Can be 0.
 tbLine - receives button line text. Can be 0.


 button index
int htb=id(9999 hWnd)
POINT p; xm p htb 1
int b=SendMessage(htb TB_HITTEST 0 &p)
if(b<1) ret

 label
if(&tbLabel)
	tbLabel.all
	Acc a=acc("" "TOOLBAR" htb "" "" 0x1000)
	a.elem=b+1; tbLabel=a.Name; err

 line
if(&tbLine)
	tbLine.all
	str s.getwintext(hWnd)
	s.getmacro(s); err goto gr
	tbLine.getl(s b)

 gr
ret b
