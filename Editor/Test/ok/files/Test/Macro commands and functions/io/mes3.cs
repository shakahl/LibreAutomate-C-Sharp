 out "%c" mes("vvvvvvvvvvv" "" "OC?2n")

MES m
m.x=-100
m.y=-1
m.style="YNC2n"
m.timeout=15
m.default='N'
 m.hwndowner=win("Notepad")

int i=mes("message" "t" m)
out i
