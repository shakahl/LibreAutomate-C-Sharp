 the script
 str imm="ExtractURL.iim"
str imm="FillForm.iim"

int w=win("- Mozilla Firefox" "MozillaWindowClass")

Acc aApp.Find(w "APPLICATION" "" "" 0x1001)
Acc aSidebar.Find(aApp.a "browser" "iMacros" "" 0x10D1 0 0 "first")
err
	Acc aButton.Find(aApp.a "PUSHBUTTON" "iMacros for Firefox" "" 0x1001)
	aButton.DoDefaultAction
	aSidebar.Find(aApp.a "browser" "iMacros" "" 0x10D1 3 0 "first")

 find the Play button in the iMacros sidebar
Acc aPanel.Find(aSidebar.a "GROUPING" "" "" 0x1044)
Acc aPlay.Find(aPanel.a "PUSHBUTTON" "Play" "" 0x1001)

 find the iMacros script in the list in the iMacros sidebar
 g1
Acc a.Find(aSidebar.a "OUTLINEITEM" imm "" 0x1011)
err
	 accessible items in closed subfolders don't exist, let the user expand the folder
	sel mes(F"Cannot find iMacros script '{imm}'. Please make it visible in the iMacros sidebar, then click Retry." "" "RC")
		case 'R' goto g1
		case else end F"Cannot find iMacros script '{imm}'"

 select and run it
a.DoDefaultAction
aPlay.DoDefaultAction

 wait until finished
rep() 0.001; if(aPlay.State&STATE_SYSTEM_UNAVAILABLE=0) break

OnScreenDisplay "finished"
