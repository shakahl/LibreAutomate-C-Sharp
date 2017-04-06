DateTime d.FromComputerTime
VARIANT v=d
outx v.vt
DateTime dd
 dd=v ;;type mismatch
dd.t=v ;;OK
out dd.ToStr

 DateTime d.FromComputerTime
 VARIANT v=d
  VARIANT v=&d
 outx v.vt
 DATE dd=v
 out dd

 DATE d.getclock
 VARIANT v=d
  VARIANT v=&d
 outx v.vt
  DateTime dd=v ;;type mismatch
 DateTime dd.t=v ;;OK
 out dd.ToStr

 DateTime d.FromComputerTime
 DATE dd=d
 out dd

 DATE dd="2000.05.05 12:55"
 out dd

 VARIANT v="2000.05.05 12:55"
 DATE dd=v
 out dd

 DATE d.getclock
 VARIANT v=d
  VARIANT v=&d
 outx v.vt
 DATE dd=v
 out dd

 DATE d.getclock
  DateTime dd=v ;;type mismatch
 DateTime dd.t=d ;;OK
 out dd.ToStr

 BSTR b="2000.05.05 12:55"
 DATE dd=b
 out dd
