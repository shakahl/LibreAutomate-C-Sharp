 out
int w=wait(3 WV win("Quick Macros Forum • Index page - Mozilla Firefox" "MozillaWindowClass"))

 PF
 Acc a.Find(w "LINK" "Untitled" "" 0x3011 3)
 PN;PO ;;849295

 PF
 Acc a.FindFF(w "A" "Untitled" "" 0x1001 3)
 PN;PO ;;199078

 PF
 UIA.IUIAutomationElement e=Uia(w "Untitled")
 PN;PO ;;109578
 out e

PF
 UIA.IUIAutomationElement e=Uia2(w 0 "Untitled") ;;107747
 UIA.IUIAutomationElement e=Uia2(w UIA.UIA_HyperlinkControlTypeId "Untitled") ;;119368
 UIA.IUIAutomationElement e=Uia2(w 0 "Untit*" 1) ;;401034
 UIA.IUIAutomationElement e=Uia2(w UIA.UIA_HyperlinkControlTypeId "Untit*" 1) ;;158614

 Acc ap.Find(w "DOCUMENT" "" "" 0x3011)
  PN
  UIA.IUIAutomationElement e=Uia3(ap 0 "Untitled") ;;6342224
  UIA.IUIAutomationElement e=Uia3(ap UIA.UIA_HyperlinkControlTypeId "Untitled") ;;6378461
  UIA.IUIAutomationElement e=Uia3(ap 0 "Untit*" 1) ;;8457457
 UIA.IUIAutomationElement e=Uia3(ap UIA.UIA_HyperlinkControlTypeId "Untit*" 1) ;;7805803

  UIA.IUIAutomationElement e=Uia4(326 72 0 "Untitled") ;;83036
  UIA.IUIAutomationElement e=Uia4(326 72 UIA.UIA_HyperlinkControlTypeId "Untitled") ;;87733
  UIA.IUIAutomationElement e=Uia4(326 72 0 "Untit*" 1) ;;266573
 UIA.IUIAutomationElement e=Uia4(326 72 UIA.UIA_HyperlinkControlTypeId "Untit*" 1) ;;130500

UIA.IUIAutomationElement p=Uia2(w UIA.UIA_DocumentControlTypeId "Quick Macros Forum • Index page") ;;22494
PN
 UIA.IUIAutomationElement e=Uia5(p 0 "Untitled") ;;77946
 UIA.IUIAutomationElement e=Uia5(p UIA.UIA_HyperlinkControlTypeId "Untitled") ;;84791
 UIA.IUIAutomationElement e=Uia5(p 0 "Untit*" 1) ;;255833
UIA.IUIAutomationElement e=Uia5(p UIA.UIA_HyperlinkControlTypeId "Untit*" 1) ;;125202

PN;PO
out e
