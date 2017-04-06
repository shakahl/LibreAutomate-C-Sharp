 /
function# iid FILTER&f

 Allows starting macro only during certain period after MC_First was run.
 
#compile MC_Init
if(!g_mc.Initialized) MC_Init

int t=GetTickCount
if(t>=g_mc.tStart and t<=g_mc.tStart+g_mc.tWait) ret iid ;;allow macro to run
ret -2 ;;don't run macro, but allow other similar triggers
