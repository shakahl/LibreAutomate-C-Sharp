int w=wait(3 WV win("Help" "HH Parent"))
Acc a.Find(w "LINK" "Menu Items and Shortcut Keys" "" 0x3001 3)
 int w=win("DebugView" "dbgviewClass")
 Acc a.Find(w "PUSHBUTTON" "Capture Kernel (Ctrl+K)" "class=ToolbarWindow32[]id=1" 0x1005)
 int w=win("Find" "#32770")
 Acc a.Find(w "PUSHBUTTON" "Find Text" "class=Button[]id=1129" 0x1005)
 int w=wait(3 WV win("Windows 7 DLL File Information - UIAutomationCore.dll - Mozilla Firefox" "MozillaWindowClass"))
 Acc a.FindFF(w "A" "Feedback" "" 0x1001 3)
out a.Name
UiaFromAcc(a)
PF
UIA.CUIAutomation u._create
PN
rep 7
	UIA.IUIAutomationElement e=u.ElementFromIAccessible(a.a a.elem) ;;very slow
	
	 u.ElementFromIAccessibleBuildCache
	 MSHTML.IHTMLElement k=+__QueryService(e uuidof(MSHTML.IHTMLElement)); out k ;;0
	PN
PO

out e
