function# needMax ARRAY(RECT)&a RECT&rs

int i j k
RECT r

if needMax
	GetClientRect m_hParent &r; SwapRECT(r); k=r.right
	for(i 0 a.len) r=a[i]; j=r.right-(r.left-rs.right); if(j<k) k=j
else
	k=0
	for(i 0 a.len) r=a[i]; j=r.left+(rs.left-r.right); if(j>k) k=j

ret k
