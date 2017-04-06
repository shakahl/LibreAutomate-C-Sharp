ifk (4)
	out 1
else
	out 0

int+ g_lastWheelTick
ifk (4)
	g_lastWheelTick=GetTickCount
	 macro1
	out 1
else
	if(GetTickCount-g_lastWheelTick<500) goto macro1
	g_lastWheelTick=0
	 macro0
	out 0
