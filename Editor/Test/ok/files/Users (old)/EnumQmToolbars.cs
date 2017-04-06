 /
function# ARRAY(int)&hwnds [hwndOwner]

dll user32 #FindWindowEx hWnd1 hWnd2 $lpsz1 $lpsz2

hwnds=0
int h
rep
	h=FindWindowEx(0 h "QM_Toolbar" 0); if(!h) break
	if(!hwndOwner or GetToolbarOwner(h)=hwndOwner) hwnds[hwnds.redim(-1)]=h

ret hwnds.len
