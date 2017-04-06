function height

 Sets list dialog box height.
 Run using mac before list.

 EXAMPLE
 str s = "Line1[]Line2[]Line3[]Line4[]Line5[]Line6[]Line7[]Line8[]Line9[]Line10[]Line11"
 mac "SetListHeight" "" 250
 sel list(s "Lines")
	 case 1 out "Line1"
	 case 2 out "Line2"
	 case 3 out "Line3"
	 case 4 out "Line4"
	 case 5 out "Line5"
	 case 6 out "Line6"
	 case 7 out "Line7"
	 case 8 out "Line8"
	 case 9 out "Line9"
	 case 10 out "Line10"
	 case 11 out "Line11"
	 case else out "Cancel"

rep 10
	int h=wait(3 WC win("" "#32770" "qm")); err ret
	int hc=child(4 "" "ListBox" h)
	if(hc) break
	0.05
if(!hc) ret

RECT r rc
GetWindowRect(h &r)
int diff=height-(r.bottom-r.top)
siz 10 r.bottom-r.top+diff h 1

GetWindowRect(hc &r)
siz 0 r.bottom-r.top+diff hc 1

hc=id(2 h)
GetWindowRect(hc &r)
GetClientRect(h &rc)
mov 0 rc.bottom-(r.bottom-r.top) hc
