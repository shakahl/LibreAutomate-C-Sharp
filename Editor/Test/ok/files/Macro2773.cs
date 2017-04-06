out
int w=win("Hung" "#32770")
 Acc a.Find(w "PUSHBUTTON" "OK" "class=Button[]id=1" 0x1005)
 Acc a.FromWindow(w)
 out a.Name

PF
UIA.IUIAutomationElement e=Uia(w)
PN
str s=e.CurrentName
PN;PO
out s
