spe 100
act "Microsoft Visual"
OnScreenDisplay "click first address" -1
wait 30 ML
OsdHide
dou
str s1.getsel; s1-"0x"
OnScreenDisplay "click next address" -1
wait 600 ML
OsdHide
dou
str s2.getsel; s2-"0x"
OnScreenDisplay _s.from(val(s2)-val(s1))
