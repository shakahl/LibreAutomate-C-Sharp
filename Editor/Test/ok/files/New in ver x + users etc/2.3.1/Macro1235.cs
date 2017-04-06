 Acc a=acc("" "COMBOBOX" win("Tipster Proofed Results - Mozilla Firefox" "MozillaUIWindowClass") "MozillaUIWindowClass" "Back This Horse" 0x1004)
 a.Mouse
 Acc a=acc("1st On Racing" "LISTITEM" win("Tipster Proofed Results - Mozilla Firefox" "MozillaUIWindowClass") "MozillaUIWindowClass" "" 0x1011)
 a.DoDefaultAction

 Acc a=acc("Tipster:" "TEXT" win("Tipster Proofed Results - Mozilla Firefox" "MozillaUIWindowClass") "MozillaUIWindowClass" "" 0x1881 0x40 0x20000040 "next2 first2")
 rep
	 str s=a.Name
	  out s
	 if(a.Name="1st On Racing")
		 a.DoDefaultAction
		 break
	 a.Navigate("next"); err break

Acc a=acc("Tipster:" "TEXT" win("Tipster Proofed Results - Mozilla Firefox" "MozillaUIWindowClass") "MozillaUIWindowClass" "" 0x1881 0x40 0x20000040 "next2")
a=acc("1st On Racing" "LISTITEM" a "" "" 16)
a.DoDefaultAction
