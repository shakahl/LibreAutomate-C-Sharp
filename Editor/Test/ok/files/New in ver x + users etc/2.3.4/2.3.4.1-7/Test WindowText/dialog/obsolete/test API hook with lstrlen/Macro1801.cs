 /exe
out
#compile "DAH_def"

int+ flstrlen

rep 3
	mac "MylstrlenThread"

rep 1000
	AH_CreateHook(&lstrlen &Mylstrlen &flstrlen)
	AH_EnableHooks
	
	lstrlen("ggg")
	
	AH_DisableHooks
	 0.001
	int t1=perf
	 rep(3) 0.001
	 0.005
	 Sleep 0
	int t2=perf
	t2-t1; t2/1000
	if(t2>=10) out t2
	AH_DeleteHooks
out "ok"

