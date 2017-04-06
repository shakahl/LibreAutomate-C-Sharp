
type MyToolbarInteractData IStringMap'm ARRAY(str)h
MyToolbarInteractData- x
x.m._create

  set hook and wait


#sub Hook
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime
if(!hwnd) ret

MyToolbarInteractData- x
 x.m.Add(...)
 ...

Acc a.FromEvent(hwnd idObject idChild)
sel a.Name
	case ["Barre d'adresse et de recherche"]
	str url=a.Value
	SendMessage toolbarwindow WM_MYMESSAGE 0 &x
	case else ret