 /
function# save str&s [$filter] [$defExt] [str&initDir] [$title] [noDereferenceLinks] [ARRAY(str)&multi] [hwndOwner] ;;save: 0 open, 1 save;  filter example: "Text files[]*.txt[]All Files[]*.*[]".

 Shows "Open" or "Save As" dialog for selecting a file.
 Returns 1 on Open/Save, 0 on Cancel.

 save - dialog type: 0 - "Open", 1 - "Save As".
 s - str variable that receives full path.
   If not empty on input:
     Sets initial content of the file name field.
     If full path, also sets initial directory.
       QM 2.3.3. Supports $special folders$.
 filter - file types displayed in dialog. Consists of description/pattern string pairs. See examples.
 defExt - default extension. The dialog uses it when the user types a filename without extension and does not select a file type from filter.
 initDir - str variable that sets initial directory and receives selected file's directory.
   The function may change the current directory, but if initDir is used, restores it when the dialog is closed.
 title - dialog box title.
 noDereferenceLinks - get path of shortcut file, not its target.
 multi - if used, Open dialog allows to select multiple files, and multi is populated with full paths. s can be 0.
 hwndOwner (QM 2.3.0) - handle of owner window. It will be disabled.
   If omitted or 0, will use the active window, if it belongs to current thread. To create unowned dialog, use GetDesktopWindow for hwndOwner.

 REMARKS
 This function fills an <google>OPENFILENAME structure</google> and calls Windows API GetOpenFileName or GetSaveFileName.

 EXAMPLES
 str s
 if OpenSaveDialog(0 s "Text files[]*.txt[]Image files[]*.bmp;*.gif[]All Files[]*.*[]")
 	out s

 ARRAY(str) a; int i
 if OpenSaveDialog(0 0 "" "" 0 "" 0 a)
	 for(i 0 a.len) out a[i]


int multisel i j k
if(&multi) multi=0; if(!save) multisel=1

BSTR b.alloc(iif(multisel 30000 300))
if(!&s) &s=_s
if(s.len) lstrcpynW b.pstr @s.expandpath b.len
s=""

OPENFILENAMEW op.lStructSize=sizeof(op)
if(!hwndOwner) hwndOwner=win; if(GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0
op.hwndOwner=hwndOwner
op.lpstrFile=b
op.nMaxFile=b.len
op.lpstrDefExt=@defExt
op.lpstrTitle=@title

op.Flags=OFN_HIDEREADONLY
if(noDereferenceLinks) op.Flags | OFN_NODEREFERENCELINKS
if(save) op.Flags | OFN_OVERWRITEPROMPT|OFN_NOREADONLYRETURN
else if(multisel) op.Flags | OFN_ALLOWMULTISELECT|OFN_EXPLORER

if(&initDir)
	initDir.searchpath
	op.lpstrInitialDir=@initDir
	str curdir=GetCurDir

str sf=filter
sf.findreplace("[]" "" 16) ;;if as separators used newline characters, replace them with null characters
sf.fromn(sf sf.len "" 1) ;;maybe forgot to append null or newline
op.lpstrFilter=@sf

if(save) i=GetSaveFileNameW(&op)
else i=GetOpenFileNameW(&op)
0
if(!i) ret

if(multisel)
	if(b[op.nFileOffset-1]) multi[].ansi(b) ;;single
	else
		str sp.ansi(b) sn
		for j op.nFileOffset b.len
			k=lstrlenW(&b[j]); if(!k) break
			sn.ansi(&b[j])
			multi[].fromn(sp sp.len "\" 1 sn sn.len)
			j+k
else s.ansi(b)

if(&initDir)
	initDir=GetCurDir
	if(curdir.len) SetCurDir curdir; err

ret 1
