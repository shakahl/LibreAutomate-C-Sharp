#compile "_PerfSM"
__SM_CATKEYS* p=_PerfSM
p.perfCounter=0
long c; QueryPerformanceCounter(&c)
p.perfArr[0]=c
