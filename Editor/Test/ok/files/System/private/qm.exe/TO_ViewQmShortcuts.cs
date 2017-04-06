 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 523 198 "QM Shortcuts"
 3 SysListView32 0x54000009 0x200 0 20 524 178 ""
 4 Static 0x54000000 0x0 2 2 518 18 "QM shortcuts in 4 folders: $desktop$, $common desktop$, $start menu$\quick macros, quick launch.[]Right click..."
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

if(!ShowDialog(dd &TO_ViewQmShortcuts 0 win)) ret

ret
 messages
int i h=id(3 hDlg)
sel message
	case WM_INITDIALOG goto initd
	case WM_COMMAND ret 1
	case WM_CONTEXTMENU if(wParam=h) goto cmenu
ret

 initd
SendMessage h LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT -1
TO_LvAddCols h "File" -60 "Command" -20 "Hotkey" -15
 g1
SendMessage h LVM_DELETEALLITEMS 0 0
str s1 s2 s3
s1=
 $desktop$\*.lnk
 $common desktop$\*.lnk
 $start menu$\quick macros\*.lnk
 $appdata$\Microsoft\Internet Explorer\Quick Launch\*.lnk

foreach s2 s1
	Dir d
	foreach(d s2 FE_Dir)
		str sPath=d.FullPath
		SHORTCUTINFO si
		if(!GetShortcutInfoEx(sPath &si)) continue
		if(!si.target.endi("\qmcl.exe") and !si.target.endi("\qm.exe")) continue
		s3=""; if(si.hotkey) FormatKeyString si.hotkey&255 si.hotkey>>8 &s3
		TO_LvAdd h -1 0 0 sPath si.param s3
ret

 cmenu
MenuPopup m.AddItems("2 Delete Shortcut(s)[]-[]3 Refresh")
ARRAY(int) a
if(!TO_LvGetSelectedItems(h a)) m.DisableItems("1 2")
i=m.Show(hDlg); if(!i) ret
sel i
	case 2 ;;Delete
	for i 0 a.len
		GetListViewItemText h a[i] s1
		del s1; err
	goto g1
	
	case 3 ;;Refresh
	goto g1
