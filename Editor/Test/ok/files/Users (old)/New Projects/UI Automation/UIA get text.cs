 sub.SetText

int w=win("Untitled - Notepad" "Notepad")
Acc a.Find(w "TEXT" "" "id=15" 0x1004)
str s=a.Value
out s.len

UIA.IUIAutomationElement e=Uia(id(15 w))
UIA.IUIAutomationValuePattern v=e.GetCurrentPattern(UIA_ValuePatternId)
str ss=v.CurrentValue
out ss.len ;;limits to 4096, same as accessible object functions


#sub SetText
str s=
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678
 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678

act "Notepad"
key Ca
paste s
