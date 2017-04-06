str f="$qm$\ok.qml"
Dir d
if(d.dir(f))
	 ok
	long t=d.TimeModified
	out _s.timeformat("" t)
	 error, type mismatch
	out _s.timeformat("" d.TimeModified)
