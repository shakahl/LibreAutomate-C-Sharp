 /Dialog_Editor

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 241 85 "Replace"
 3 Edit 0x54231044 0x204 26 4 212 22 ""
 4 Edit 0x54231044 0x204 26 28 212 22 ""
 6 Button 0x54012003 0x4 6 52 54 13 "Match case"
 5 Button 0x54012003 0x4 62 52 56 13 "Whole word"
 9 Button 0x54012003 0x4 120 52 42 13 "Regexp."
 11 Button 0x54032000 0x4 164 52 20 14 "R&X"
 12 Button 0x54032000 0x4 198 52 24 14 "Save"
 13 Button 0x54032000 0x4 222 52 16 14 "..."
 7 Button 0x54032009 0x4 146 68 40 13 "All text"
 8 Button 0x54012009 0x4 188 68 50 13 "Selection"
 10 Button 0x54030001 0x4 6 68 52 14 "&Replace All"
 2 Button 0x54030000 0x4 62 68 40 14 "Close"
 14 Button 0x54032000 0x4 106 68 34 14 "Test"
 15 Static 0x54020000 0x4 4 30 20 13 "To"
 16 Static 0x54020000 0x4 4 6 20 12 "From"
 END DIALOG
 DIALOG EDITOR: "" 0x2010200 "" ""

function# hDlg message wParam lParam

if(sub_to.ToolDlgCommon(&hDlg "[][]ReplaceAll")) ret wParam
sel message
	case WM_INITDIALOG
	int- t_hDlg; t_hDlg=hDlg
	case WM_COMMAND goto messages2
ret
 messages2
int-- t_lastFocus
sel wParam
	case 14 int test=1; goto g1
	
	case 10
	 g1
	int hwnd=GetWindowLong(hDlg GWL_HWNDPARENT)
	int n=ReplaceReplace(+DT_GetControls(hDlg) hwnd test)
	if(!n or test) act hDlg
	_s=n; _s.setwintext(id(14 hDlg))
	
	case 11
	CheckDlgButton(hDlg 9 1)
	if(!t_lastFocus) t_lastFocus=3
	int hedit=id(t_lastFocus hDlg)
	SetFocus hedit
	RegExpMenu hedit t_lastFocus=4
	
	case [EN_KILLFOCUS<<16|3,EN_KILLFOCUS<<16|4] t_lastFocus=wParam&7
	
	case 12
	str s.getwintext(id(3 hDlg))
	str ss.getwintext(id(4 hDlg))
	DATE d.getclock
	str sss.from(d "[]From:[]" s "[]To:[]" ss "[][]")
	File f.Open("$my qm$\Replace.txt" "a")
	f.Write(sss)
	
	case 13 run "$my qm$\Replace.txt"; err
ret 1
