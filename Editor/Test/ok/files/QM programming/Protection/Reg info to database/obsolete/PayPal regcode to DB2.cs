 For Thunderbird with "Thunderbird Conversations" extension.

str s email rc

int w=win(" Thunderbird" "MozillaWindowClass")

Acc ae.Find(w "LIST" "" "class=MozillaWindowClass" 0x1084)
ae.Find(ae.a "LISTITEM" "Meto*" "" 0x10C1)
s=ae.Name
 out s
if(findrx(s "^Meto (\S+)" 0 0 email 1)<0) ret

Acc ar=acc("\s*\w{32}[\-=].+" "TEXT" ae "" "" 0x1802 0x40 0x20000040)
rc=ar.Name; rc.trim

err+ OnScreenDisplay "failed, try again"; ret

RegcodeToDb rc email
