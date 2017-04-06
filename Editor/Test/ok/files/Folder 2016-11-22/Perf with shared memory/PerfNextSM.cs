#compile "_PerfSM"
__SM_CATKEYS* p=_PerfSM
if p.perfCounter<10
	long c; QueryPerformanceCounter(&c)
	p.perfCounter+1
	p.perfArr[p.perfCounter]=c
