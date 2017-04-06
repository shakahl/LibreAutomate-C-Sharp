out
int w=win("Untitled - Notepad" "Notepad")
 w=id(15 w)
 int w=win("Microsoft Spy++ - [Windows 1]" "Afx:*" "" 0x4)
 w=id(59393 w)

 out DpiIsWindowScaled(w); ret

mac("Function252" "" w)
int h=mac("Function252" "" w)
rep
	if(WaitForSingleObject(h 0)!=WAIT_TIMEOUT) break
	if(DpiIsWindowScaled(w)) out "scaled"
	 if(!DpiIsWindowScaled(w)) out "not scaled"
