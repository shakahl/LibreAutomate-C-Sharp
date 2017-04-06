function WINDOWINFO&w [str&string]

if(!hwnd) end ERR_INIT

WINDOWINFO ww; if(!&w) &w=&ww

w.hwnd=hwnd
w.text.getwintext(hwnd)
w.class.getwinclass(hwnd)
w.programpath.getwinexe(hwnd 1)
w.program.getfilename(w.programpath)
w.style=GetWindowLong(hwnd GWL_STYLE)
w.exstyle=GetWindowLong(hwnd GWL_EXSTYLE)
if(w.style&WS_CHILD)
	w.id=GetWindowLong(hwnd GWL_ID)
	w.hwndparent=GetParent(hwnd)
	w.hwndtopparent=GetAncestor(hwnd 2)
	w.hwndowner=0
else
	w.id=0; w.hwndparent=0; w.hwndtopparent=0
	w.hwndowner=GetWindow(hwnd GW_OWNER)
GetWindowRect hwnd &w.rectangle

if(&string)
	if(w.style&WS_CHILD) lpstr styledescr=" (child)"
	else if(w.style&WS_POPUP) styledescr=" (popup)"

	string.format("Handle: %i[]Text: %s[]Class: %s[]Program: %s (%s)[]Styles: 0x%X 0x%X%s[]Rectangle: left=%i top=%i width=%i height=%i" hwnd w.text w.class w.program w.programpath w.style w.exstyle styledescr w.rectangle.left w.rectangle.top w.rectangle.right-w.rectangle.left w.rectangle.bottom-w.rectangle.top) 
	if(w.style&WS_CHILD) string.formata("[]id: %i[]Parent: %i[]Top-level parent: %i" w.id w.hwndparent w.hwndtopparent)
	else string.formata("[]Owner: %i" w.hwndowner)
