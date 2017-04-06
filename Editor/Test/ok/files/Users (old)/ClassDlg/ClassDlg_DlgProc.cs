 \Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 10 10 48 14 "Button"
 4 Button 0x54012003 0x0 170 10 48 12 "Check"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

if(message=WM_INITDIALOG) DT_Init(hDlg lParam)
ClassDlg* p=+DT_GetParam(hDlg)
if(p) p.DlgProc(hDlg message wParam lParam)
