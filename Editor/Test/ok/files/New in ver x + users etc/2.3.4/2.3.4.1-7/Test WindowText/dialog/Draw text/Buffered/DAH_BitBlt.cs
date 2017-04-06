 /dlg_apihook
function hdc

 SetViewportOrgEx(hdc 20 20 0)

 RECT rc; GetClipBox hdc &rc; zRECT rc

RECT r; SetRect &r 0 0 300 800
 RECT r; SetRect &r 0 0 150 800
__MemBmp m.Create(r.right r.bottom)
FillRect m.dc &r GetStockObject(WHITE_BRUSH)


 SetViewportOrgEx(m.dc 20 20 0)
 RECT b; if(GetBoundsRect(m.dc &b 0)) zRECT b
 RECT b; if(GetClipBox(m.dc &b)) zRECT b
 RECT b; TO_GetBitmapRect(m.bm &b); zRECT b
 out GetCurrentObject(hdc OBJ_BITMAP)
 RECT b; TO_GetBitmapRect(GetCurrentObject(hdc OBJ_BITMAP) &b); zRECT b
 RECT b; TO_GetBitmapRect(GetCurrentObject(m.dc OBJ_BITMAP) &b); zRECT b


DAH_DrawMain m.dc

 r.bottom/3
 r.right/2.2

BitBlt hdc 0 0 r.right r.bottom m.dc 0 0 SRCCOPY
 BitBlt hdc 5 5 r.right r.bottom m.dc 0 0 SRCCOPY
 BitBlt hdc 5 5 r.right r.bottom m.dc 100 100 SRCCOPY
 BitBlt hdc 0 -105 r.right r.bottom m.dc 0 0 SRCCOPY
 BitBlt hdc 0 0 r.right+100 r.bottom m.dc 0 -10 SRCCOPY

 StretchBlt hdc 30 30 r.right*1.5 r.bottom*2 m.dc 0 0 r.right r.bottom SRCCOPY

 MaskBlt hdc 0 0 r.right r.bottom m.dc 0 0 0 0 0 SRCCOPY

 GdiTransparentBlt hdc 30 30 r.right*1.5 r.bottom*2 m.dc 0 0 r.right r.bottom 0xffffff

 BLENDFUNCTION bf.SourceConstantAlpha=255
  GdiAlphaBlend(hdc 0 0 r.right r.bottom m.dc 0 0 r.right r.bottom bf)
 GdiAlphaBlend hdc 30 30 r.right*1.5 r.bottom*2 m.dc 0 0 r.right r.bottom bf
