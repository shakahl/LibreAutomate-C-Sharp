 /dlg_apihook
function# caller hdc @*txt cchText RECT&_r ;;caller: 1 MyDrawTextExW, 2 MyExtTextOutW, 3 MyTextOutW, 4 MyPoliTextOutW, 5 GdipDrawString, 6 GdipDrawDriverString, 9 ScriptShape, <0 ANSI

if(!cchText) ret

if caller<0
	lpstr txtA=+txt
	str u.unicode(txtA 0 iif(cchText<0 len(txtA) cchText))
	txt=+u; cchText=u.len/2
else if(cchText<0) cchText=lstrlenW(txt)

RECT r=_r
if(r.right<r.left) r.right=r.left
if(r.bottom<r.top) r.bottom=r.top
sel caller
	case [1,6] ;;RECT always specified
	case else

RECT rs
int hwnd=DAH_GetRectInScreen(hdc r rs)

str sa.ansi(txt _unicode cchText)
 str s.format("<%i>  hdc=%i nc=%i  '%s'  {%i %i %i %i}  %s" caller hdc cchText sa r.left r.top r.right-r.left r.bottom-r.top _s.outw(hwnd))
 str s.format("<%i>  hdc=%i nc=%i  '%s'  {%i %i %i %i}  %s" caller hdc cchText sa r.left r.top r.right r.bottom _s.outw(hwnd))
str s.format("<%i>  hdc=%i nc=%i  '%s'  {%i %i %i %i}  {%i %i %i %i}  %s" caller hdc cchText sa r.left r.top r.right r.bottom rs.left rs.top rs.right rs.bottom _s.outw(hwnd))

int- t_out; if(t_out) out s

WT_DrawRect hdc r r 0 0



 notes:
 Main text drawing functions:
   DrawTextExW. Callers: DrawTextW, DrawShadowText, DrawThemeText, DrawThemeTextEx.
   ExtTextOutW. Callers: TextOutW (Win7), TabbedTextOutW, DrawStatusTextW.
   TextOutW (XP).
   PolyTextOutW.
   GdipDrawString.
   GdipDrawDriverString.
 ExtTextOutW: If caller does not use ETO_IGNORELANGUAGE flag, ExtTextOutW calls itself with this flag.
 DrawTextExW calls ExtTextOutW 2 times with other strings.
 Many controls use double buffering, eg themed buttons, scintilla. At first draws to memory DC, therefore WindowFromDC returns 0. Then calls BitBlt, and we then can get the window and coordinates.
 DrawShadowText calls DrawTextExW 2 times. First time hwnd 0; ignore it.
 Tested on: 7, XP SP2, 2000.
