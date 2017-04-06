 /
function# Exstyle $Class $Text Style x y width height HwndParent Id [Hfont]

 Creates child window and sets font.

 Hfont - font handle. If 0, uses default dialog font.
 Other parameters are the same as with <google>CreateWindowEx</google>.

 REMARKS
 The function always adds WS_CHILD|WS_VISIBLE styles.


int retry
 g1
int h=CreateWindowExW(Exstyle @Class @Text (Style|WS_CHILD|WS_VISIBLE) x y width height HwndParent Id _hinst 0)
if !h
	if !retry and GetLastError=ERROR_CANNOT_FIND_WND_CLASS
		if(__CDD_LoadControlDllOnDemand(Class)) retry=1; goto g1
	ret
SendMessage(h WM_SETFONT iif(Hfont Hfont _hfont) 0)
 note: SysIPAddress32 bug: deletes font when destroying.
ret h
