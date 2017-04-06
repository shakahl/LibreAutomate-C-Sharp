\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 179 317 "QM 4-1-1"
 3 Edit 0x54030080 0x200 9 9 96 14 ""
 4 Edit 0x54030080 0x200 9 27 96 14 ""
 5 Edit 0x54030080 0x200 9 45 96 14 ""
 6 Edit 0x54030080 0x200 9 63 96 14 ""
 7 Edit 0x54030080 0x200 9 81 96 14 ""
 8 Static 0x54000000 0x0 108 12 48 13 "First"
 9 Static 0x54000000 0x0 108 30 48 12 "Last"
 10 Static 0x54000000 0x0 108 48 48 12 "City"
 11 Static 0x54000000 0x0 108 66 48 12 "State"
 12 Static 0x54000000 0x0 108 84 48 13 "Zip"
 17 Button 0x54032000 0x0 9 99 72 21 "Get Information"
 13 Edit 0x54231044 0x200 9 123 148 183 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030105 "" "" ""
str controls = "3 4 5 6 7"
str e3 e4 e5 e6 e7
e3="Bill"
e4="Gates"
if(!ShowDialog("QM_411" &QM_411 &controls)) ret

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 17
	str First.getwintext(id(3 win("QM 4-1-1" "#32770")))
	str Last.getwintext(id(4 win("QM 4-1-1" "#32770")))
	str State.getwintext(id(6 win("QM 4-1-1" "#32770")))
	str City.getwintext(id(5 win("QM 4-1-1" "#32770")))
	str Zip.getwintext(id(7 win("QM 4-1-1" "#32770")))
	ARRAY(str) Information
	AnyWho_Lookup Information First Last State City Zip

	str sInformation=Information
	sInformation.setwintext(id(13 win("QM 4-1-1" "#32770")))
	
	case IDOK
	case IDCANCEL
ret 1