int+ g_myinfowindow
if(!IsWindow(g_myinfowindow)) g_myinfowindow=InfoWindow(-1 1 0 "QM - InfoWindow")

int v1 v2 v3
v1=4
v2=3
v3=2
str s=
F
 Case1 - {v1} times
 Case2 - {v2} times
 Case3 - {v3} times
s.setwintext(g_myinfowindow)
