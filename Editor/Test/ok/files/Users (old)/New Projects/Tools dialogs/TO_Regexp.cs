 /Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 To make the dialog respond to user input (eg, execute some code
 when a button is pressed), edit this function - add more case
 statements to handle messages. Included are several commented
 samples for often used messages.
 You should not remove existing code (except comments) because it
 is required to properly initialize and uninitialize the dialog.


str controls = "3 6 7 11 13 15 17 18"
str e3Reg e6 e7 cb11Fun e13Sub lb15Opt e17Res c18Dec
if(!ShowDialog("TO_Regexp" &TO_Regexp &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 327 190 "Find or replace using regular expression"
 1 Button 0x54030001 0x4 4 172 48 14 "OK"
 2 Button 0x54030000 0x4 54 172 48 14 "Cancel"
 3 Edit 0x54030080 0x200 4 4 318 14 "Reg"
 4 Button 0x54032000 0x0 4 22 40 14 "Menu"
 6 Edit 0x54030880 0x20000 108 24 214 12 ""
 5 Static 0x54020000 0x0 4 46 40 12 "Test string"
 7 Edit 0x54231044 0x200 48 44 274 48 ""
 8 Button 0x54032000 0x0 4 78 40 14 "Find"
 10 Static 0x54020000 0x0 6 112 38 12 "Function"
 11 ComboBox 0x54230243 0x0 48 108 86 199 "Fun"
 12 Static 0x54020000 0x0 6 128 82 12 "Subject string (variable)"
 13 Edit 0x54030080 0x200 90 126 44 14 "Sub"
 14 Static 0x54020000 0x0 166 108 30 13 "Options"
 15 ListBox 0x54230109 0x200 198 106 124 34 "Opt"
 16 Static 0x54020000 0x0 6 146 52 12 "Result variable"
 17 Edit 0x54030080 0x200 60 144 44 14 "Res"
 18 Button 0x54012003 0x0 106 144 44 12 "Declare"
 20 Button 0x54032000 0x0 104 172 18 14 "?"
 19 Static 0x54000010 0x20004 4 164 320 2 ""
 9 Static 0x54000010 0x20004 4 100 320 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010200 "*" ""

ret
 messages
sel message
	case WM_COMMAND
	sel wParam
		case IDOK ret TO_Ok(hDlg)
	ret 1
