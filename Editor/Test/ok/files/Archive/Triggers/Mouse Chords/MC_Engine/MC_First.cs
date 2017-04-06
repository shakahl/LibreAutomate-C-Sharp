 /
function# iid FILTER&f

 Runs when you press the first button in a chord.
 To change first button in chords, change trigger of this function.
 If there are more macros that have same trigger as this function,
 this function must be above them in the list.

#compile MC_Init
if(!g_mc.Initialized) MC_Init

int t=GetTickCount
if(g_mc.onDoubleClick and t>=g_mc.tStart and t<=g_mc.tStart+g_mc.tDoubleClick) ;;the first button was double clicked
	mac g_mc.onDoubleClick; err ;;runs macro that is set in MC_Init
	 or, you can convert double click to single click:
	 mid ;;clicks the middle button. You can enable this line and disable the mac line.
else g_mc.tStart=t
ret -1
