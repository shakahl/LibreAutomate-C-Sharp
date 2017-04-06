 /
function# iid FILTER&f

 Makes the macro specific to a particular mouse.
 For more info, read "Keyboard Detector Help" macro.
 

#compile Keyboard_Detector
 
if(!g_ri.mworking or !g_rir.m[3]) ret -2
SendMessage g_ri.hwnd 0 0 0
int mb=iif(f.tkey<9 f.tkey 0)
if(g_ri.mb!=mb) 0.001
if(g_ri.mb=mb)
	if(g_ri.mouse_id!=g_rir.m[3]) ret -2
else
	if(g_ri.m[3]!=mb) ret -2
	int t=GetTickCount-g_ri.mt[3]
	if(t>1000 or t<0) ret -2
g_ri.m[3]=0; ret iid
