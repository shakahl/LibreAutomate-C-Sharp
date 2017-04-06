#compile "__Gdip"

int hwnd=win

RECT r; GetClientRect hwnd &r
POINT p; ClientToScreen hwnd &p

__MemBmp mb
mb.Create(r.right r.bottom 1 p.x p.y)

_s="new_img"
if(OpenSaveDialog(1 _s "JPG files[]*.jpg[]All Files[]*.*[]" ".jpg"))
	GdipBitmap im
	if(!im.FromHBITMAP(mb.bm)) end "error"
	if(!im.Save(_s)) end "error"
	run _s

