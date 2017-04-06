
int reverse=iif(val(e5)>=15 128 0)
Acc u.Find(w3 "BUTTON" e5 "" 0x3001|reverse)
u.DoDefaultAction; 0.15


#ret
Try:
Let htm find date "1" and then get another day using the navig parameter.
If it is difficult with htm, try to use accessible objects.

,Htm e47=htm("BUTTON" "1" "" w3 "0" 85 0x21 50 val(e5)-1)
