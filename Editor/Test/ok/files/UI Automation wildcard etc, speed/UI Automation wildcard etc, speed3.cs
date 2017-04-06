 out
int w=win("Notepad" "#32770")

PF
Acc a.Find(w "STATICTEXT" "Do you want to save changes to Untitled?" "class=DirectUIHWND" 0x1005)
PN;PO ;;7000

PF
 UIA.IUIAutomationElement e=Uia2(w 0 "Do you want to save changes to Untitled?") ;;20023
 UIA.IUIAutomationElement e=Uia2(w UIA.UIA_TextControlTypeId "Do you want to save changes to Untitled?") ;;20571
 UIA.IUIAutomationElement e=Uia2(w 0 "Do you want to save*" 1) ;;36279
UIA.IUIAutomationElement e=Uia2(w UIA.UIA_TextControlTypeId "Do you want to save*" 1) ;;37227

PN;PO
out e
