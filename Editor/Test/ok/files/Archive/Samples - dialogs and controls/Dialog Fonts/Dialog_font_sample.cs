\Dialog_Editor

 Shows how to set fonts and colors.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "6 3"
str lb6 e3
e3="Text"
lb6="item1[]item2"
if(!ShowDialog("Dialog_font_sample" &Dialog_font_sample &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 173 99 "Dialog Fonts"
 4 Static 0x54000000 0x4 10 22 48 14 "Text"
 6 ListBox 0x54230101 0x200 104 20 60 34 ""
 3 Edit 0x54030080 0x204 10 38 82 16 ""
 1 Button 0x54030001 0x4 4 82 48 14 "OK"
 2 Button 0x54030000 0x4 56 82 48 14 "Cancel"
 5 Button 0x54020007 0x4 4 6 166 54 "Text"
 END DIALOG
 DIALOG EDITOR: "" 0x2020103 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__Font-- f
	f.Create("Courier New" 14 1)
	f.SetDialogFont(hDlg "3-5")
	DT_SetTextColor(hDlg 0xff0000 "3 4")
	
	__Font-- f2
	f2.Create("Comic Sans MS" 12 2)
	f2.SetDialogFont(hDlg "2")
	
	__Font-- f3
	DT_SetTextColor(hDlg 0x008000 "6")
	
	 note: __Font variables must not be local because they must exist while the dialog is open.
	 note: in a dialog, use different __Font variables for different fonts.

	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
