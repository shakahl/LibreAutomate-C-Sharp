 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 1004 1005 1008 1102 1103 1106 1105 6 7"
str lb3Act e1004Pat c1005Var e1008Fil e1102pv c1103Wit e1106 c1105Wit e6Var c7Dec

lb3Act="&Search for file and get full path[]Search for file in folder and get full path[]Expand $special folder$ or %environment variable% in string[]Extract path part from string variable containing full path[]Extract filename part from string variable containing full path"

if(!ShowDialog("TO_Path" &TO_Path &controls)) ret

str s



 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 257 158 "File path operations"
 3 ListBox 0x54230101 0x204 4 4 250 46 "Act"
 8 Static 0x54020000 0x4 106 58 150 12 "Leave empty if path is stored in result variable"
 1006 Static 0x54020000 0x4 6 58 24 12 "Folder"
 1001 Static 0x54020000 0x4 6 58 20 12 "Path"
 1004 Edit 0x54030080 0x204 4 70 250 14 "Pat"
 1002 Button 0x54032000 0x4 4 86 48 14 "Browse ..."
 1003 Button 0x54032000 0x4 54 86 18 14 "SF"
 1005 Button 0x54012003 0x4 78 86 42 12 "Variable"
 1007 Static 0x54020000 0x4 134 88 34 12 "Filename"
 1008 Edit 0x54030080 0x204 170 86 84 14 "Fil"
 1101 Static 0x44020000 0x4 6 58 48 12 "Path (variable)"
 1102 Edit 0x44030080 0x204 60 56 38 14 "pv"
 1103 Button 0x44012003 0x4 6 82 70 13 "With extension"
 1104 Static 0x44020000 0x4 82 84 50 13 "Append \ and"
 1106 Edit 0x44030080 0x204 134 82 120 14 ""
 1105 Button 0x44012003 0x4 6 82 48 13 "With \"
 1 Button 0x54030001 0x4 4 140 48 14 "OK"
 2 Button 0x54030000 0x4 54 140 48 14 "Cancel"
 4 Button 0x54032000 0x4 104 140 18 14 "?"
 5 Static 0x54020000 0x4 6 114 52 12 "Result variable"
 6 Edit 0x54030080 0x204 60 112 38 14 "Var"
 7 Button 0x54012003 0x4 100 112 48 12 "Declare"
 15 Static 0x54000010 0x20004 4 104 250 1 ""
 11 Static 0x54000010 0x20004 4 132 250 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010200 "*" "0"

ret
 messages
sel message
	case WM_INITDIALOG
	goto g11
	case WM_COMMAND
	sel wParam
		case IDOK ret TO_Ok(hDlg)
		case 1002 if(TO_Browse2(s "pathdir")) s.setwintext(id(1004 hDlg)); TO_Check hDlg "1005" 0
		case 1003 TO_SF hDlg 1004 "\"
		 ----
		case LBN_SELCHANGE<<16|3
		int i=LB_SelectedItem(lParam)
		DT_Page hDlg i "0 0 0 1 1"
		 g11
		if(i<=2) TO_Show hDlg "1006 1007 1008" i=1
		else TO_Show hDlg "1103" i=3; TO_Show hDlg "1104-1106" i=4
		TO_Show hDlg "8" i!=1
	ret 1
