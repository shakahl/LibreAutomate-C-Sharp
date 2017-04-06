 int w=win("" "QM_Editor")
int w=win("Firefox" "Mozilla*WindowClass" "" 0x4)
if(!w) run "firefox.exe"; wait 15 " - Mozilla Firefox"; act _hwndqm; ret

 Acc a.Find(w "LINK" "Quick Macros" "" 0x3011)
Acc a.FindFF(w "SELECT" "" "id=sk" 0x1004)
 Acc a.FindFF(w "A" "Security Advisories" "" 0x1081)
 Acc a.Find(w "LINK" "Privatumo politika" "" 0x3091)

out a.Name
0.5
clo "Firefox"



#ret
 rep 100
	 if(flags&0x2000) AccessibleObjectFromWindow(hwnd OBJID_CLIENT uuidof(IAccessible) &ad.a)
	 else ad=acc("" "" hwnd "" "" 0x2000)
	 if(ad.a) break
	 int retry; retry+1
 if(retry) out retry
