int+ g_18243
_i=GetTickCount
int isDouble=GetTickCount-g_18243<500; g_18243=GetTickCount

if isDouble
	out "double"
else
	out "single"
