DateTime d dNow

d.FromStr("2013-02-01")
d.AddParts(10) ;;add 10 days

dNow.FromComputerTime

if d>dNow
	out "d is in future"
else
	out "action"
