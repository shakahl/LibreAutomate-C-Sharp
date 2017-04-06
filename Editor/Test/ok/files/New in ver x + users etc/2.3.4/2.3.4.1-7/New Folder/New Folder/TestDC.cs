 /dlg_test_dc
function hdc isWindow

out
if(isWindow) ret
 out "%i %i" hdc isWindow

 POINT p
  if(GetViewportOrgEx(hdc &p)) out p.y
  if(GetWindowOrgEx(hdc &p)) out p.y
 if(GetDCOrgEx(hdc &p)) out p.x

 RECT r
 if(GetBoundsRect(hdc &r 0)) zRECT r

 out GetCurrentObject(hdc OBJ_BITMAP)

int hrgn=CreateRectRgn(0 0 0 0)
 out hrgn

 out GetMetaRgn(hdc hrgn)
 out GetClipRgn(hdc hrgn)
 out GetRandomRgn(hdc hrgn SYSRGN)

RECT rv; SetRect &rv 100 100 10 10

Q &q
 RectVisible(hdc &rv)
 Q &qq

RECT r rr
int c=GetClipBox(hdc &r)
Q &qq

if GetRandomRgn(hdc hrgn SYSRGN)
	int k=GetRgnBox(hrgn &r)
	if k=COMPLEXREGION
		int nb=GetRegionData(hrgn 0 0)
		 out nb
		RGNDATA* rd=+LocalAlloc(0 nb)
		if(GetRegionData(hrgn nb rd))
			out rd.rdh.nCount
		LocalFree rd
Q &qqq
outq

zRECT r
zRECT rr

DeleteObject hrgn

#ret
NULLREGION Region is empty. 
SIMPLEREGION Region is a single rectangle. 
COMPLEXREGION 
