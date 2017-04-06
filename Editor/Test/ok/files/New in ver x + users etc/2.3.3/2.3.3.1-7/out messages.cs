 Converts message values to names.
 Copy list of messages and run this macro.
 A line in the list must be in this format:
 .+?: (-?\d+) (-?\d+) (-?\d+)

out "---------- MESSAGES ----------"
str s ss.getclip
ARRAY(str) a
foreach s ss
	if(findrx(s ".+?: (-?\d+) (-?\d+) (-?\d+)" 0 0 a)<0) out s; continue
	int m=val(a[1])
	 sel(m) case [] continue
	OutWinMsg m val(a[2]) val(a[3])
