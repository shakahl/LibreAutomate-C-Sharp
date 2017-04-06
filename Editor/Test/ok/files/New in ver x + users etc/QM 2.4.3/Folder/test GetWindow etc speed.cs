out

 __Tcc x.Compile("" "_EnumProc@8" 0 "user32")
__Tcc x.Compile("" "GW" 0 "user32")

PF
 int w=win("TBTEST" "QM_tb_test")
int h
 h=GetTopWindow(0)
 outw2 h; outx GetWinStyle(h 1)&WS_EX_TOPMOST
 outw2 GetWindow(h GW_HWNDFIRST)
 outw2 GetWindow(_hwndqm GW_HWNDFIRST)
 outw2 GetWindow(GetShellWindow GW_HWNDFIRST)
 outw2 FindWindowEx(0 0 0 0)

 ARRAY(int)+ g_aw; if(!g_aw.len) win "" "" "" 0 "" g_aw
 out g_aw.len

int i
 for i 0 g_aw.len
rep 1
	 h=GetTopWindow(0)
	 EnumWindows(&sub.Proc 0)
	 EnumWindows(x.f 0)
	 EnumDesktopWindows(0 &sub.Proc 0)
	 ARRAY(int) a; win "" "" "" 0 "" a
	
	 h=GetTopWindow(0); rep() if(h) h=GetWindow(h GW_HWNDNEXT); else break
	 h=GetTopWindow(0)
	 h=_hwndqm
	 h=g_aw[i]
	_i=call(x.f 0 GW_HWNDNEXT)
	 break
PN
PO
out _i


#sub Proc
function# hwnd param
 outw2 hwnd
ret 1


#ret
#include <windows.h>

int __stdcall EnumProc(int hwnd, int param)
{
//printf("%i", hwnd);
return 1;
}

int GW(HWND h, int gw)
{
if(!h) h=GetTopWindow(0); if(gw==GW_HWNDPREV) h=GetWindow(h, GW_HWNDLAST);
int i=0;
while(h) { h=GetWindow(h, gw); i++; if(i>10000) break; }
return i;
}

