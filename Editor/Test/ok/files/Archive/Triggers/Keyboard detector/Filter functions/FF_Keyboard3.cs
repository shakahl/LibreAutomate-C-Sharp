 /
function# iid FILTER&f

 Makes the macro specific to a particular keyboard.
 For more info, read "Keyboard Detector Help" macro.
 

#compile Keyboard_Detector

if(!g_ri.kworking or !g_rir.k[2]) ret -2
SendMessage g_ri.hwnd 0 0 0
if(g_ri.vk!=f.tkey) 0.001
if(g_ri.vk=f.tkey)
	if(g_ri.keyboard_id!=g_rir.k[2]) ret -2
else
	if(g_ri.k[2]!=f.tkey) ret -2
	int t=GetTickCount-g_ri.kt[2]
	if(t>1000 or t<0) ret -2
g_ri.k[2]=0; ret iid
