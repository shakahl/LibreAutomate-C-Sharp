 Acc a=acc("Minimize" "PUSHBUTTON" win("" "Notepad") "" "" 0x1001)
 Acc a=acc("Paste" "PUSHBUTTON" win("Text" "#32770") "Button" "" 0x1001)
 Acc a=acc("Google Paieška" "PUSHBUTTON" win("Google - Mozilla Firefox" "MozillaUIWindowClass") "MozillaUIWindowClass" "" 0x1081) ;;does not change state
 Acc a=acc("Google Paieška" "PUSHBUTTON" win("Google - Windows Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1001) ;;does not change state
 Acc a=acc("Settings..." "PUSHBUTTON" win("Options" "bosa_sdm_Microsoft Office Word 11.0") "bosa_sdm_Microsoft Office Word 11.0" "" 0x1001) ;;does not change state
 Acc a=acc("Text" "PUSHBUTTON" win("QM TOOLBAR" "QM_toolbar") "ToolbarWindow32" "" 0x1001)
 Acc a=acc("Untitled - Notepad" "PUSHBUTTON" win("" "Shell_TrayWnd") "MSTaskListWClass" "" 0x1001)
 Acc a=acc("Triggers" "PAGETAB" win("Options" "#32770") "SysTabControl32" "" 0x1001)
 Acc a=acc("Trigger" "COLUMNHEADER" win("QM - My Macros - All Programs" "#32770") "SysHeader32" "" 0x1001)
Acc a=acc("Members: erase category" "CHECKBUTTON" win("Options" "#32770") "Button" "" 0x1001)

a.WaitForPressed()
rep
	outx a.State
	1