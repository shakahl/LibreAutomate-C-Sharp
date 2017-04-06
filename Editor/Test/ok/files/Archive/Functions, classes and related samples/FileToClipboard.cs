 /
function! $fileList

 Stores file path to the clipboard in CF_HDROP clipboard format.
 Returns 1 if successful, 0 if not.

 fileList - file path, or several file paths in multiple lines.

 EXAMPLE
 if(!FileToClipboard("$my qm$\x1.txt[]$my qm$\x2.txt")) end "failed"


str sf sfl
foreach sf fileList
	sf.expandpath
	sf.unicode
	sfl.fromn(sfl sfl.len sf sf.len+2)

if(!OpenClipboard(0)) ret
EmptyClipboard

int gh=GlobalAlloc(GMEM_MOVEABLE|GMEM_ZEROINIT sizeof(DROPFILES)+sfl.len+2)
DROPFILES* f=+GlobalLock(gh)
f.pFiles=sizeof(DROPFILES)
f.fWide=1
memcpy(f+sizeof(DROPFILES) sfl sfl.len)
GlobalUnlock(gh)

int r=SetClipboardData(CF_HDROP gh)!0
CloseClipboard
ret r
