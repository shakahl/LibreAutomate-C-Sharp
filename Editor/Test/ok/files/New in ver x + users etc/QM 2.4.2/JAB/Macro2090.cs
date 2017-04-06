 int w=wait(3 WV win("Quick Macros - automation software for Windows. Macro program. Keyboard, mouse, record, toolbar - Internet Explorer" "IEFrame"))
 PF
 Acc a.Find(w "LINK" "Resources" "" 0x3001 3)
 PN;PO

 int w=wait(3 WV win("Tryit Editor v2.0 - Google Chrome" "Chrome_WidgetWin_1"))
 PF
 Acc a.Find(w "COMBOBOX" "" "value=Volvo" 0x3004 3)
 PN;PO

 int w=wait(3 WV win("Quick Macros - automation software for Windows. Macro program. Keyboard, mouse, record, toolbars. - Mozilla Firefox" "Mozilla*WindowClass" "" 0x4))
 PF
 Acc a.Find(w "LINK" "Resources" "" 0x3011 3)
 PN;PO

 int w=win("SwingSet2" "CabinetWClass")
 PF
 Acc a.Find(w "OUTLINEITEM" "Network" "class=SysTreeView32[]id=100" 0x1005)
 PN;PO

int w=win("Global Options jEdit: General" "SunAwtDialog")
PF
Acc a.Find(w "check box" "Use default locale" "" 0x1001)
PN;PO
