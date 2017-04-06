 /dlg_apihook
function# hdc POLYTEXTW*ppt nstrings

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnPolyTextOutW hdc ppt nstrings)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(!R) ret R

RECT r
int i
for i 0 nstrings
	POLYTEXTW& k=ppt[i]
	r.left=k.x; r.top=k.y; r.right=k.x; r.bottom=k.y
	if(k.uiFlags&ETO_CLIPPED)
		if(k.rcl.right>r.right) r.right=k.rcl.right
		if(k.rcl.bottom>r.bottom) r.bottom=k.rcl.bottom
	
	CommonTextFunc 4 hdc k.lpstr k.n &r

ret R
