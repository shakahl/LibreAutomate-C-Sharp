\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str cb3
if(!ShowDialog("Excel_Sheets_Dialog" &Excel_Sheets_Dialog &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ComboBox 0x54230243 0x0 8 8 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 0 1
Excel.Application app
Excel.Worksheet ws
sel wParam
	case CBN_SELENDOK<<16|3
	_i=CB_SelectedItem(lParam)
	app._getactive
	ws=app.Worksheets.Item(_i+1)
	ws.Activate
	
	case CBN_DROPDOWN<<16|3
	_i=CB_SelectedItem(lParam)
	app._getactive
	str s
	foreach ws app.Worksheets
		s+ws.Name; s+"[]"
	TO_CBFill lParam s
	
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
