
 Initializes C# script engine.
 Optional. Other functions implicitly call this function if still not initialized.


if(x) ret

x=CreateCsScript_3

if !__cs_sett.i
	lock __cs_sett
	if !__cs_sett.i
		__cs_sett.i=1
		if(__cs_sett.s.len) x.SetOptions(1 __cs_sett.s); __cs_sett.s.all

err+ end _error
