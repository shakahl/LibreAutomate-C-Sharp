int+ g_18244
if(g_18244) g_18244=0; ret
int isDouble=1
g_18244=1; wait 0.5 -V g_18244; err isDouble=0; g_18244=0

if isDouble
	out "double"
else
	out "single"

 if this is a macro, check "Run simultaneously" in Properties
