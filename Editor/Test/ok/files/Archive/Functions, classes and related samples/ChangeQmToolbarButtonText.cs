 /
function $tbname button $text

 Changes QM toolbar button text.
 Returns 1 if successful, 0 if not.
 Note: The original text is restored when you change toolbar style through the right-click menu.

 tbname - toolbar name.
 button - 0-based line index in toolbar text.
 text - new text.


str sn=tbname
int h=win(sn.ucase "QM_toolbar"); if(!h) ret
h=id(9999 h)
TBBUTTONINFOW ti.cbSize=sizeof(ti)
ti.dwMask=TBIF_TEXT
ti.pszText=@text
ret SendMessage(h TB_SETBUTTONINFOW button &ti)
