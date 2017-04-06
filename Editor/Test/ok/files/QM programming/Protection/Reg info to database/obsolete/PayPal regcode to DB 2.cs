str s email rc

int w1=win(" Thunderbird" "MozillaUIWindowClass")

Acc ae.FindFF(w1 "label" "" "id=expandedtoLabel[]value=''to ''" 0x3084 0 0 "next first2")
s=ae.Name
 out s
if(findrx(s "^to:.+<(.+)>$" 0 0 email 1)<0)
	if(findrx(s "^to: *(.+@.+\..+)$" 0 0 email 1)<0) ret

Acc ar=acc("\s*\w{32}[\-=].+" "TEXT" w1 "MozillaContentWindowClass" "" 0x1802 0x40 0x20000040)
rc=ar.Name; rc.trim

RegcodeToDb rc email
