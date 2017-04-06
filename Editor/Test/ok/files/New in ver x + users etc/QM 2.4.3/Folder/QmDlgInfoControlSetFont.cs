 /
function hwnd [$fontName] [fontSize]

 Changes font of control of class QM_DlgInfo.
 Call on WM_INITDIALOG. Set control text after.

 hwnd - control handle.
 fontName - font name, or 0 to not change.
 fontSize - font size, or 0 to not change.


int h=hwnd
if(!empty(fontName)) SendMessage(h SCI.SCI_STYLESETFONT 32 fontName)
if(fontSize) SendMessage(h SCI.SCI_STYLESETSIZE 32 fontSize)

 save <code> styles
type __QDISTYLE colText colBack !bold !italic !underline !eol
ARRAY(__QDISTYLE) a.create(32)
int i
for i 1 18
	__QDISTYLE& r=a[i]
	r.colText=SendMessage(h SCI.SCI_STYLEGETFORE i 0)
	r.colBack=SendMessage(h SCI.SCI_STYLEGETBACK i 0)
	r.bold=SendMessage(h SCI.SCI_STYLEGETBOLD i 0)
	r.italic=SendMessage(h SCI.SCI_STYLEGETITALIC i 0)
	r.underline=SendMessage(h SCI.SCI_STYLEGETUNDERLINE i 0)
	r.eol=SendMessage(h SCI.SCI_STYLEGETEOLFILLED i 0)

 set other styles the same as the default style 32. It clears <code> styles etc, that is why we save/restore them.
SendMessage(h SCI.SCI_STYLECLEARALL 0 0)

 restore
for i 1 18
	&r=a[i]
	SendMessage(h SCI.SCI_STYLESETFORE i r.colText)
	SendMessage(h SCI.SCI_STYLESETBACK i r.colBack)
	SendMessage(h SCI.SCI_STYLESETBOLD i r.bold)
	SendMessage(h SCI.SCI_STYLESETITALIC i r.italic)
	SendMessage(h SCI.SCI_STYLESETUNDERLINE i r.underline)
	SendMessage(h SCI.SCI_STYLESETEOLFILLED i r.eol)
SendMessage(h SCI.SCI_STYLESETVISIBLE 31 0)
