 /
function# hDlg idEdit $regdir [$defdir] [$filter] [$defext] [str&s] [idLnkParam]

if(empty(defdir)) defdir="$documents$"
if(empty(filter)) filter="All Files[]*.*[]"
if(!&s) &s=_s
int f=sub_to.File_GetPathFlags(hDlg)
str ss

rget ss regdir "\Tools" 0 defdir
 g1
if(!OpenSaveDialog(0 s.all filter defext ss 0 1)) ret
str se.expandpath(ss 2)
rset se regdir "\Tools"

SHORTCUTINFO si
if(s.endi(".lnk") and GetShortcutInfoEx(s &si) and FileExists(si.target 1)) ss=si.target; goto g1 ;;folder shortcut

if(f&1=0) sub_to.File_LinkTarget s 0 hDlg idLnkParam
if(f&2=0) s.expandpath(s 2)
if(idEdit) TO_SetText s hDlg idEdit
ret 1

 used in Archive
