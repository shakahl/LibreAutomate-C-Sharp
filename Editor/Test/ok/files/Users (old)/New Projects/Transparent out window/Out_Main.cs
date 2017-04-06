type OUT_VAR hwnd hwndedit caller
OUT_VAR- v
v.caller=getopt(itemid 1)
int pos sz; rget(pos "out pos"); rget(sz "out size" "" 0 150<<16|300)
int x(pos&0xffff) y(pos>>16) cx(sz&0xffff) cy(sz>>16)
if(x<-cx/2) x=-cx/2
if(x>ScreenWidth-(cx/2)) x=ScreenWidth-(cx/2)
if(y<0) y=0
if(y>ScreenHeight-(cy/2)) y=ScreenHeight-(cy/2)
_s.getmacro(getopt(itemid 3) 1); _s-"QM rt output  -  "
ret MainWindow(_s "QM_Out_Class" &Out_WndProc x y cx cy WS_OVERLAPPEDWINDOW|WS_CLIPCHILDREN 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST|WS_EX_LAYERED)

