 /
function hDlg `image

 Sets dialog background image.

 hDlg - dialog handle.
 image - picture file or bitmap handle.
   File can be bmp, jpg or gif. QM 2.3.4: also can be png.
   Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.
   If handle, you can delete the bitmap after calling this function.
   If 0, clears previous image or color/gradient.
   Draws tiled.

 Added in: QM 2.3.3.


__DIALOGCOLORS* p=sub_DT.Colors(hDlg)

p.hBrush.Delete
int hb
sel image.vt
	case VT_BSTR __GdiHandle _hb=LoadPictureFile(image); hb=_hb
	case else hb=image.lVal
if(hb) p.bkFlags=3; p.hBrush=CreatePatternBrush(hb)
else p.bkFlags=0

if(IsWindowVisible(hDlg)) InvalidateRect hDlg 0 1
