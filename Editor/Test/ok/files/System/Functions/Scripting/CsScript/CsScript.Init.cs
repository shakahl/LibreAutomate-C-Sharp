
 Initializes this variable.
 Optional. Other functions implicitly call this function if need. Does nothing if already called.
 First time this function is quite slow, because loads .NET runtime and QM C# dll.


if(x) ret

opt noerrorshere 1
opt nowarningshere 1

int hr d
hr=CreateCsScript(&x &d)
if hr
	str e
	if(hr=0x80131700) e="Must be installed .NET framework version 4.x or 3.5."
	else e.dllerror("" "" hr); if(!e.len) e=F"0x{hr}"
	end F"{ERR_FAILED} to initialize. {e}    d={d}"

if !__cs_sett.i
	lock __cs_sett
	if !__cs_sett.i
		__cs_sett.i=1
		if(__cs_sett.s.len) x.SetOptions(1 __cs_sett.s); __cs_sett.s.all

 def CLR_E_SHIM_RUNTIMELOAD 0x80131700
