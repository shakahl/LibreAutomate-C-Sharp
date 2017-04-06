function [timeMS]

 Uses CPU timeMS ms, default 100.
 Call before measuring code speed (<help>PerfFirst</help>), to make CPU work at full speed.


if(!timeMS) timeMS=100
int n
int t0=timeGetTime; rep() n+1; if(timeGetTime-t0>=timeMS) break; else rep(100) _i+1
 out n
