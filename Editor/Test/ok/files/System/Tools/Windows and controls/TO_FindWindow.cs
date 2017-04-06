 \Dialog_Editor
function! [flags] [hwndOwner] [idEdit] [str&sHwnd] [str&sAll] ;;flags: 1 no owner (default qm), 2 act qm, 4 no InsertStatement

 Shows 'Find window or control' dialog.
 Formats win/child/id statement's, declares variables.
 Inserts the code, unlass flag 4.
 Returns 1 on OK, 0 on Cancel.

 hwndOwner - owner window. If not used, uses _hwndqm, unless flag 1.
 idEdit - edit control. If used, receives handle.
 sHwnd - if used, receives window handle variable.
 sAll - if used, receives all code.

str dd=
 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 247 130 "Find window or control"
 7 QM_Tools 0x54030000 0x10000 4 4 240 54 "1 49"
 1 Button 0x54030001 0x4 4 112 48 14 "OK"
 2 Button 0x54010000 0x4 54 112 50 14 "Cancel"
 8 Button 0x54032000 0x4 106 112 16 14 "?"
 6 QM_DlgInfo 0x54000000 0x20000 4 64 240 38 "Blank fields - gets handle of the active window or focused control.[][]Tip: Use Ctrl+Shift+Alt+W to record window/control from mouse or to show this dialog anywhere."
 3 Static 0x54000010 0x20000 0 106 254 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030401 "*" "" "" ""

str controls = "7"
__strt qmt7

if(!hwndOwner) hwndOwner=iif(flags&1 0 _hwndqm)
int style; if(!hwndOwner and GetWinStyle(win 1)&WS_EX_TOPMOST) style|DS_SYSMODAL
if(!ShowDialog(dd &sub.Dlg &controls hwndOwner 0 style 0 3)) ret
if(flags&2 and IsWindowVisible(_hwndqm)) act _hwndqm; err

str s _sHwnd
__strt vd
int ty=qmt7.Win(s); if(!ty) ret
if(!&sHwnd) &sHwnd=_sHwnd
s=F"{vd.VD(_s.from(`-i int ` iif(ty=1 `w` `c`)) sHwnd)}={s}"
qmt7.WinEnd(s)
if(&sAll) sAll=s

if(idEdit) TO_SetText sHwnd hwndOwner idEdit 4
if(flags&4=0) InsertStatement s
ret 1


#sub Dlg
function# hDlg message wParam lParam

 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\window_find.ico[]TO_FindWindow" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 8 QmHelp "IDP_CHILD[]IDP_WIN" !but(512 hDlg)
ret 1
