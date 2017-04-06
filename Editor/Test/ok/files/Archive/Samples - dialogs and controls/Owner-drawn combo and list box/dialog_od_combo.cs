\Dialog_Editor

 Shows how to draw icons and bitmaps in combo box and list box controls.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6"
str cb3 cb4 lb5 lb6
cb3="&one[]two[]three"
cb4="&one[]two[]three"
lb5="&one[]two[]three"
lb6="&one[]two[13]lines and with word wrap[]three"
if(!ShowDialog("dialog_od_combo" &dialog_od_combo &controls)) ret
out cb3
out cb4
out lb5
out lb6

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 204 164 "Dialog"
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 56 146 48 14 "Cancel"
 3 ComboBox 0x54230253 0x0 4 6 96 215 ""
 4 ComboBox 0x54230252 0x0 4 26 96 215 ""
 5 ListBox 0x54230151 0x200 4 104 96 36 ""
 6 ListBox 0x54230151 0x200 110 6 90 134 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages

CB_DrawImages hDlg message wParam lParam 3 "mouse.ico[]qm.exe[]qm.exe,3"
CB_DrawImages hDlg message wParam lParam 4 "shell32.dll,0[]shell32.dll,1[]shell32.dll,2" 0 32 32 DT_VCENTER|DT_SINGLELINE
CB_DrawImages hDlg message wParam lParam 5 "keyboard.ico[]qm.exe,1[]qm.exe,4" 1
CB_DrawImages hDlg message wParam lParam 6 "$My Pictures$\60x90.bmp[]$My Pictures$\60x90.jpg[]$My Pictures$\60x90.gif" 1|16 60 90 DT_WORDBREAK

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
