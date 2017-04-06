out
int w=wait(3 WV win("Mozilla Firefox" "MozillaWindowClass"))
 int w=win("Google Chrome" "Chrome_WidgetWin_1")
 int w=wait(3 WV win("Internet Explorer" "IEFrame"))

act w
key F5
0.2

Acc a.Find(w "DOCUMENT" "" "" 0x3010 3)
 Acc a.Find(w "PANE" "" "" 0x3000 3)
a.State(_s); out F"Acc: {_s}"

UIA.IUIAutomationElement e=UiaFromAcc(a)
out e.CurrentItemStatus
