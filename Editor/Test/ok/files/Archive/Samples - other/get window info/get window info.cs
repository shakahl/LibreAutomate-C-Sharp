int h=child(mouse)
if(!h) h=win(mouse); int toplevel=1

str txt, cls, exe, exefull, styledescr
int style, exstyle, idc
RECT r

txt.getwintext(h)
cls.getwinclass(h)
exefull.getwinexe(h 1)
exe.getfilename(exefull)
style=GetWindowLong(h GWL_STYLE)
exstyle=GetWindowLong(h GWL_EXSTYLE)
GetWindowRect h &r
if(!toplevel)
	idc=GetWindowLong(h GWL_ID)
	styledescr=" (child)"
else if(style&WS_POPUP) styledescr=" (popup)"
 what else?

str data_under_mouse.format("Handle: %i[]Text: %s[]Class: %s[]Program: %s (%s)[]Styles: 0x%X 0x%X%s[]Rectangle: left=%i top=%i width=%i height=%i" h txt cls exe exefull style exstyle styledescr r.left r.top r.right-r.left r.bottom-r.top) 
if(!toplevel) data_under_mouse.formata("[]id: %i[]Parent: %i" idc GetParent(h))

out data_under_mouse
