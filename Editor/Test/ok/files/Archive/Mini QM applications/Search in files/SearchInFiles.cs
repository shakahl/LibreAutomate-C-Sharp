\Dialog_Editor
function# [hDlg] [message] [wParam] [lParam]
if(hDlg) goto messages

type DLG_SEARCHINFILES ~controls ~e4fol ~c13Inc ~e6fil ~e8tex ~c12Reg ~c15Mat ~c29Sea ~c18Dou ~cb25dat ~lb10lis ~e20rep ~c21Onl
	!insearch !stop
DLG_SEARCHINFILES d.controls="4 13 6 8 12 15 29 18 25 10 20 21"
d.c13Inc=1
d.cb25dat="&Modified[]Created"
GetFolderPathInExplorer d.e4fol
_monitor=-2
ShowDialog("SearchInFiles" &SearchInFiles &d)

 BEGIN DIALOG
 0 "" 0x90CA0A48 0x100 0 0 396 318 "Search in files"
 3 Static 0x54000000 0x0 4 4 26 13 "Look in"
 11 Button 0x54032000 0x0 32 4 16 14 "..."
 4 Edit 0x54030080 0x200 50 4 264 14 "fol"
 13 Button 0x54012003 0x0 318 4 76 13 "Include subfolders"
 5 Static 0x54000000 0x0 4 22 42 12 "File pattern"
 6 Edit 0x54030080 0x200 50 22 130 14 "fil"
 14 Static 0x54000000 0x0 184 22 210 12 "Use * and ?. Use | as separator. Example: *.txt|*.ini"
 7 Static 0x54000000 0x0 4 40 42 12 "Text in file"
 8 Edit 0x54231044 0x200 50 40 264 34 "tex"
 12 Button 0x54012003 0x0 318 40 54 10 "Regexp"
 17 Button 0x54032000 0x0 374 38 20 14 "RX"
 15 Button 0x54012003 0x0 318 52 54 10 "Match case"
 29 Button 0x54012003 0x0 318 64 62 10 "Search binary"
 9 Button 0x54032000 0x0 4 74 42 14 "Search"
 16 Button 0x5C032000 0x0 4 90 42 14 "Stop"
 18 Button 0x54012003 0x0 58 94 100 10 "Double click opens folder"
 28 Static 0x54000000 0x0 244 79 46 10 "Date"
 25 ComboBox 0x54230243 0x0 242 92 54 213 "dat"
 23 Static 0x54000000 0x0 296 79 18 10 "from"
 26 SysDateTimePick32 0x54000002 0x200 316 78 76 14 ""
 24 Static 0x54000000 0x0 304 94 10 9 "to"
 27 SysDateTimePick32 0x54000002 0x200 316 92 76 14 ""
 10 ListBox 0x54330109 0x200 0 108 398 176 "lis"
 22 Static 0x54000000 0x0 4 290 44 26 "Replace found text to"
 20 Edit 0x54231044 0x200 50 287 264 29 "rep"
 19 Button 0x5C032000 0x0 318 302 50 14 "Replace..."
 21 Button 0x54012003 0x0 318 287 62 13 "Only selected"
 END DIALOG
 DIALOG EDITOR: "DLG_SEARCHINFILES" 0x2020008 "*" ""

ret
 messages
DLG_SEARCHINFILES* p=+DT_GetVariables(hDlg)
sel message
	case WM_INITDIALOG
	QmRegisterDropTarget hDlg 0 48
	SHAutoComplete id(4 hDlg) 0
	for(_i 26 28) SendDlgItemMessage hDlg _i DTM_SETSYSTEMTIME GDT_NONE 0
	
	case WM_DESTROY
	rep(100) if(p.insearch) p.stop=1; opt waitmsg 1; 0.1; else break
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP TO_DropFiles hDlg +lParam
ret
 messages2
sel wParam
	case [1,9] ;;Search
	mac "SearchInFilesThread" "" hDlg
	ret
	case 16 ;;Stop
	p.stop=1
	case 19 ;;Replace
	mac "SearchInFilesThread2" "" hDlg
	case 11 if(BrowseForFolder(_s)) _s.setwintext(id(4 hDlg))
	case 17 but+ id(12 hDlg); RegExpMenu id(8 hDlg)
	
	case LBN_DBLCLK<<16|10
	_i=LB_SelectedItem(lParam)
	if(!LB_GetItemText(lParam _i _s)) ret
	_s-p.e4fol
	if(but(18 hDlg)) run "explorer.exe" F"/select,{_s}"; err
	else run _s; err
ret 1

