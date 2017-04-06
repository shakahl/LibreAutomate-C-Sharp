 out
int w=win("Google Chrome" "Chrome_WidgetWin_1")
if(!w) run "$local appdata$\Google\Chrome\Application\chrome.exe"; 1; w=wait(0 WV win("Google Chrome" "Chrome_WidgetWin_1")); 5
outw w
outw child("" "Chrome_WidgetWin_1" w)

int c=child("" "Chrome_RenderWidgetHostHWND" w) ;;document
outw c
ret
  int c=child("" "Chrome_WidgetWin_0" w) ;;document
 outw c
  Acc aDoc.Find(w "DOCUMENT")
  outw child(aDoc.a)
  ret
Acc aClient
aClient.FromWindow(c OBJID_CLIENT)
if(!aClient.ChildCount)
	SendMessage(c WM_GETOBJECT OBJID_CLIENT 1)
	rep 10
		0.01
		 outw c
		aClient.FromWindow(c OBJID_CLIENT); out aClient.a
		int st=aClient.State(_s); err out "ERROR"; continue
		out _s
		out aClient.ChildCount
		if(!(st&STATE_SYSTEM_BUSY)) break

Acc a.Find(aClient.a "PUSHBUTTON" "" "" 0x3001 0)
out a.Name
