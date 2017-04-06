 the script
 str imm="ExtractURL.iim"
 str imm="FillForm.iim"
 str imm="Open6Tabs.iim"
str imm="Filter.iim"

int w=win("- Mozilla Firefox" "MozillaWindowClass")

 find the Play button in the iMacros sidebar
Acc aPlay.Find(w "PUSHBUTTON" "Play" "" 0x1011)

 find and select the iMacros script in the list in the iMacros sidebar
Acc a.Find(w "OUTLINEITEM" imm "" 0x1011)
a.DoDefaultAction ;;selects

 run it
aPlay.DoDefaultAction

 wait until finished
rep() 0.02; if(aPlay.State&STATE_SYSTEM_UNAVAILABLE=0) break

mes "finished"
