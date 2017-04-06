 Runs Excel Help from Tips pane link.

str s
s=_command
if(!s.len) s="Application.Calculate" ;;testing

spe 10
int w=win("Microsoft Visual Basic - " "wndclass_desked_gsk" "EXCEL")
if !w
	int w1=win("Microsoft Excel - " "XLMAIN")
	act w1
	key AF11
	w=wait(5 win("Microsoft Visual Basic - " "wndclass_desked_gsk" "EXCEL"))
act w
int c=child("(Code)" "VbaWindow" w)
if(!c) key F7; c=wait(5 WC child("(Code)" "VbaWindow" w))
act c
key CaX (s) F1
 SendKeysToWindow c key(Y (s) F1) ;;Excel crashes
 int w2=wait(30 WA win("Microsoft Visual Basic Help" "MsoHelp11"))
hid w
 act _hwndqm
 act w2
