str s="text wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww"

 at first show OSD to get coordinates
OnScreenDisplay s 0 0 0 0 0 0 1 "osd_chameleon"
int hWnd=win("osd_chameleon" "QM_OSD_Class")
RECT r; GetClientRect hWnd &r
POINT p; ClientToScreen hWnd &p
spe; hid hWnd;
clo hWnd

 create and save bitmap
__MemBmp mb
mb.Create(r.right r.bottom 1 p.x p.y)
SaveBitmap mb.bm "$temp$\osd_chameleon.bmp"

 then show another OSD with that bitmap
OnScreenDisplay s 0 0 0 0 0 0 1 0 0 0 "$temp$\osd_chameleon.bmp"
