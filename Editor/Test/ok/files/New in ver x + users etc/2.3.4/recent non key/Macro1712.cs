out
DateTime d
VARIANT v

DATE a.getclock
DateTime b.FromComputerTime
out b
str s=a

 v=a
v=b
 v=s

if __VarToDateTime(v d)
	out d.ToStr
