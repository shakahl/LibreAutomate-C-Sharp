 /dlg_apihook
function hdc CLogicalCoord&c

SetBkMode hdc TRANSPARENT
int R
 goto g1

  int hrgn=CreateRectRgn(10 0 180 100)
  int hrgn=CreateRectRgn(0 0 0 0)
 int hrgn=CreateEllipticRgn(0 0 180 100)
  int hrgn=CreateEllipticRgn(100 0 180 50)
  int hrgn=CreateEllipticRgn(100 0 150 30)
  OffsetRgn hrgn -30 0
 SelectClipRgn hdc hrgn
 DeleteObject hrgn
 
 RECT b
 if(GetClipBox(hdc &b)) zRECT b
  int hr=CreateRectRgn(0 0 0 0)
  if GetRandomRgn(hdc hr SYSRGN)
	  out GetRgnBox(hdc &b);;>1) zRECT b
  DeleteObject hr
 
 R=DrawTextExW(hdc L"CALL DrawTextExW b" -1 +&c 0 0)
 
  int hr=CreateRectRgn(0, 0, 0, 0)
  out "------------ %i  %i" hdc GetClipRgn(hdc, hr)
  DeleteObject hr
 _________________________________________

R=DrawTextExW(hdc L"&MULTILINE1[]multiline2." -1 +&c 0 0); c.Offset(0 40)
R=DrawTextExW(hdc L"Nomultiline1,[13]nomultiline2" -1 +&c DT_SINGLELINE 0); c.Offset(0 40)
R=DrawTextExW(hdc L"Tabs1[9]Tabs2" -1 +&c DT_EXPANDTABS 0); c.Offset(0 40)
R=DrawTextExW(hdc L"Notabs1,[9]notabs2" -1 +&c 0 0); c.Offset(0 40)
DRAWTEXTPARAMS dt.cbSize=sizeof(dt); dt.iTabLength=10; R=DrawTextExW(hdc L"Tabs3,[9]Tabs4" -1 +&c DT_EXPANDTABS|DT_TABSTOP &dt); c.Offset(0 40)

c.Init(hdc 0 200 100 100)
R=DrawTextExW(hdc L"WRAP1 WRAP2 WRAP3" -1 +&c DT_WORDBREAK 0); c.Offset(0 40)
R=DrawTextExW(hdc L"WRAP1 WRAP2            WRAP3" -1 +&c DT_WORDBREAK 0); c.Offset(0 40)
R=DrawTextExW(hdc L"TTTTTabs5[9]TTTTTabs6" -1 +&c DT_EXPANDTABS|DT_WORDBREAK 0); c.Offset(0 40)

R=DrawTextExW(hdc L"&CALL&&Draw&TextExW WWWWWWWWWWWWWWWWWWWWWWWWWWWWW" -1 +&c 0 0); c.Offset(0 40)
R=DrawTextExW(hdc L"CALL DrawTextExW WWWWWWWWWWWWWWWWWWWWWWWWWWWWW" -1 +&c DT_END_ELLIPSIS 0); c.Offset(0 40)

 R=DrawTextExW(hdc L"&" -1 +&c 0 0); c.Offset(0 40)
 R=DrawTextExW(hdc L"[]" -1 +&c 0 0); c.Offset(0 40)
 R=DrawTextExW(hdc L"" -1 +&c 0 0); c.Offset(0 40)

_i=&DrawShadowText; err _i=0 ;;XP
if _i
	R=DrawShadowText(hdc L"CALL DrawShadowText" 19 +&c 0 0xff0000 0xff00 5 5); c.Offset(0 40)

DrawTextExW(hdc L"A[]B" -1 +&c 0 0); c.Offset(8 0); DrawTextExW(hdc L"A[]B" -1 +&c 0 0); c.Offset(0 40)
 _________________________________________

 REMOVING DUPLICATE AND SHADOW TEXT
 g1
R=DrawTextExW(hdc L"A" -1 +&c 0 0); c.Offset(0 40)
c.Offset(2 2); SetTextColor(hdc 0x808080); DrawTextExW(hdc L"Shadow" -1 +&c 0 0); c.Offset(-2 -2); SetTextColor(hdc 0); DrawTextExW(hdc L"Shadow" -1 +&c 0 0); c.Offset(0 40)
DrawTextExW(hdc L"Duplicate" -1 +&c 0 0); DrawTextExW(hdc L"Duplicate" -1 +&c 0 0); c.Offset(0 40)
DrawTextExW(hdc L"Duplicate AAA" -1 +&c 0 0); DrawTextExW(hdc L"Duplicate" -1 +&c 0 0); c.Offset(0 40)
DrawTextExW(hdc L"Duplicate" -1 +&c 0 0); DrawTextExW(hdc L"Duplicate AAA" -1 +&c 0 0); c.Offset(0 40)
 _________________________________________

 EMPTY TEXT RECT
RECT r; SetRect &r 0 c.y 0 c.y+50
R=DrawTextExW(hdc L"abcd" -1 &r 0 0); OffsetRect &r 0 30
R=DrawTextExW(hdc L"a[][]b" -1 &r 0 0); c.Offset(0 40); OffsetRect &r 0 30
ExtTextOutW(hdc r.left r.top ETO_CLIPPED &r L"efgh" 4 0); OffsetRect &r 0 30
r.right+10; ExtTextOutW(hdc r.left r.top 0 &r L"[]" 2 0); OffsetRect &r 0 30
