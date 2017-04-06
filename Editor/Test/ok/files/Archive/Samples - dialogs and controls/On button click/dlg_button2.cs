\Dialog_Editor

 Shows how to run code on button click, after closing the dialog.
 Note that it is not a smart dialog.

sel ShowDialog("dlg_button2" 0)
	case 1
	out "OK"
	
	case 3
	out "Button3"
	
	case 4
	out "Button4"
	
	case else
	ret

 3 and 4 are button id, as specified in text in BEGIN DIALOG ... END DIALOG


 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 18 16 48 14 "Button3"
 4 Button 0x54032000 0x0 18 36 48 14 "Button4"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""
