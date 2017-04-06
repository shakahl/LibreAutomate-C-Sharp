function h

if(!h) h=win(mouse)

 1. Use WM_GETTITLEBARINFOEX. Works. 80 mks.

 type TITLEBARINFOEX cbSize RECT'rcTitleBar rgstate[6] RECT'rgrect[6]
 TITLEBARINFOEX ti.cbSize=sizeof(TITLEBARINFOEX)
 def WM_GETTITLEBARINFOEX 0x033F
 Q &q
 SendMessage h WM_GETTITLEBARINFOEX 0 &ti
 Q &qq
 outq
 RECT r=ti.rgrect[5]
 out "%i %i %i %i" r.left r.top r.right r.bottom

 2. Use DwmDefWindowProc. Does not work.

 dll dwmapi #DwmDefWindowProc hwnd msg wParam lParam *plResult
 
 int lpar=ym<<16|xm
 int ht
 Q &q
 if(DwmDefWindowProc(h WM_NCHITTEST 0 lpar &ht)) ret
 Q &qq
 out ht

 3. Use accessible objects. Works. 250 mks.

 Q &q
 Acc a=acc(mouse)
 int r=a.Role
 a.a=0
 Q &qq
 outq
 out "0x%X" r

 4. Use WM_GETTITLEBARINFOEX and DefWindowProc. Works. 60 mks. This one now is used in qmhook20.dll.

type TITLEBARINFOEX cbSize RECT'rcTitleBar rgstate[6] RECT'rgrect[6]
TITLEBARINFOEX ti.cbSize=sizeof(TITLEBARINFOEX)
def WM_GETTITLEBARINFOEX 0x033F
Q &q
DefWindowProc h WM_GETTITLEBARINFOEX 0 &ti
Q &qq
outq
out "0x%X" ti.rgstate[0]
RECT r=ti.rgrect[3]
 RECT r=ti.rcTitleBar
out "%i %i %i %i" r.left r.top r.right r.bottom
