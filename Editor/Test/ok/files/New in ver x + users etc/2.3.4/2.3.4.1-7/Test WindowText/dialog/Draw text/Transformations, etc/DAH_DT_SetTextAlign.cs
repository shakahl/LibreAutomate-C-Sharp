 /dlg_apihook
function hdc

 SetBkMode hdc TRANSPARENT
int x(120) y
int LH=30
RECT r; SetRect &r x y 300 LH
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iLeftMargin=20; dt.iRightMargin=10
int R showR


BSTR s="CALL ExtTextOutW"
R=ExtTextOutW(hdc x y 0 0 s s.len 0); if(showR) out R
y+LH
SetTextAlign hdc TA_RIGHT
R=ExtTextOutW(hdc x y 0 0 s s.len 0); if(showR) out R
y+LH
SetTextAlign hdc TA_CENTER
R=ExtTextOutW(hdc x y 0 0 s s.len 0); if(showR) out R
y+LH
SetTextAlign hdc TA_BOTTOM
R=ExtTextOutW(hdc x y 0 0 s s.len 0); if(showR) out R
y+LH
SetTextAlign hdc TA_BASELINE
R=ExtTextOutW(hdc x y 0 0 s s.len 0); if(showR) out R
y+LH

SetTextAlign hdc TA_LEFT
OffsetRect &r 0 y+LH
R=DrawTextExW(hdc L"CALL DrawTextExW" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
SetTextAlign hdc TA_RIGHT
R=DrawTextExW(hdc L"CALL DrawTextExW r" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE[]multiline r" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
SetTextAlign hdc TA_CENTER
R=DrawTextExW(hdc L"CALL DrawTextExW c" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE[]multiline c" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH*2
SetTextAlign hdc TA_BOTTOM
R=DrawTextExW(hdc L"CALL DrawTextExW b" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE[]multiline b" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
SetTextAlign hdc TA_BASELINE
R=DrawTextExW(hdc L"CALL DrawTextExW bl" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE[]multiline bl" -1 &r DT_NOCLIP 0); OffsetRect &r 0 LH
SetTextAlign hdc TA_RIGHT
R=DrawTextExW(hdc L"CALL DrawTextExW r dt" -1 &r DT_NOCLIP &dt); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE[]multiline r dt" -1 &r DT_NOCLIP &dt); OffsetRect &r 0 LH
SetTextAlign hdc TA_CENTER
R=DrawTextExW(hdc L"CALL DrawTextExW c dt" -1 &r DT_NOCLIP &dt); OffsetRect &r 0 LH
R=DrawTextExW(hdc L"DTE[]multiline c dt" -1 &r DT_NOCLIP &dt); OffsetRect &r 0 LH
 DAH_GDIP hdc r.top ;;does not apply text align
 OffsetRect &r 0 LH*2

 SetTextAlign hdc TA_LEFT
 
 s="Buffered paint"
 SetRect &r 0 r.bottom 300 r.bottom+100
 int hdc2 hbp=BeginBufferedPaint(hdc &r 0 0 &hdc2)
 y=r.top
 R=ExtTextOutW(hdc2 x y 0 0 s s.len 0); if(showR) out R
 y+LH
 SetTextAlign hdc2 TA_RIGHT
 R=ExtTextOutW(hdc2 x y 0 0 s s.len 0); if(showR) out R
 y+LH
 EndBufferedPaint(hbp 1)
