 /
function $s SIZE&z [hFont] [dtFlags]

 Calculates text size for display on screen or in a control.

 s - text. Can be multiline.
 z - variable that receives text width and height.
 hFont - font handle. If 0 or omitted, uses default dialog font.
 dtFlags - flags for DrawText (look in MSDN library).
   These flags can be useful: DT_EXPANDTABS (include tabs), DT_NOPREFIX (include &), DT_SINGLELINE (ignore newlines)

 REMARKS
 Uses DrawText API with DT_CALCRECT flag and screen device context.


RECT r
int dc oldfont
dc=CreateCompatibleDC(0); oldfont=SelectObject(dc iif(hFont hFont _hfont))

if(DrawTextW(dc @s -1 &r dtFlags|DT_CALCRECT)) z.cx=r.right-r.left; z.cy=r.bottom-r.top
else z.cx=0; z.cy=0

SelectObject(dc oldfont); DeleteDC(dc)
