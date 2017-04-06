function [stop]

int+ g_stop_N0
if(stop) g_stop_N0=1; ret
g_stop_N0=0

 out "begin"
spe 1
opt keymark 1
rep 100
	if(g_stop_N0) break
	key N0
 out "end"
