 /dlg_apihook
function hdc

int sdc=SaveDC(hdc)

__Font- t_f.Create("Segoe UI" 10)
SelectObject(hdc t_f)
int mx(1) my(1)

 SetViewportOrgEx(hdc 30 30 0)
 SetViewportOrgEx(hdc -30 -30 0)
 SetWindowOrgEx(hdc -100 -100 0)
 SetTextCharacterExtra hdc 5
 SetTextJustification hdc 50 2 ;;does not change anything
 SetTextAlign hdc TA_CENTER; SetViewportOrgEx(hdc 70 0 0) ;;TA_RIGHT TA_CENTER TA_BOTTOM TA_BASELINE
 SetMapMode hdc MM_TWIPS; __Font f.Create("Arial" 200); SelectObject(hdc f)
 SetMapMode hdc MM_LOMETRIC; __Font f.Create("Arial" 40); SelectObject(hdc f)
 SetMapMode hdc MM_HIMETRIC; __Font f.Create("Arial" 400); SelectObject(hdc f)
 SetMapMode hdc MM_LOENGLISH; __Font f.Create("Arial" 14); SelectObject(hdc f)
 SetMapMode hdc MM_HIENGLISH; __Font f.Create("Arial" 140); SelectObject(hdc f)
 SetMapMode hdc MM_ANISOTROPIC; SetViewportExtEx(hdc 2 2 0)
 SetMapMode hdc MM_ANISOTROPIC; SetViewportExtEx(hdc 1 -1 0); SetViewportOrgEx(hdc 0 -600 0)
 SetMapMode hdc MM_ANISOTROPIC; SetViewportExtEx(hdc -1 1 0);; SetViewportOrgEx(hdc -300 0 0)
 SetMapMode hdc MM_ISOTROPIC; SetViewportExtEx(hdc 1 1 0) ;;does not draw TO, only DT
 SetGraphicsMode(hdc GM_ADVANCED); XFORM f; f.eM11=1; f.eM22=-1; SetWorldTransform(hdc &f) ;;reflect (flip vertically)

CLogicalCoord c.Init(hdc 0 0 300 30)
 CLogicalCoord c.Init(hdc 0 30 300 -30) ;;use with world transform

DAH_DT_Single hdc c
 DAH_DrawText hdc c
 DAH_DrawTextA hdc c
 DAH_Uniscribe hdc c
 DAH_DT_SetTextAlign hdc
 DAH_DT_SetViewportExtEx hdc
 DAH_DT_SetMapMode hdc
 DAH_DT_SetWorldTransform hdc
 DAH_DT_CurrentPosition hdc
 DAH_DT_Misc hdc c
 DAH_GDIP hdc c
 DAH_DT_MiscBlt hdc c
 DAH_DT_Exception hdc c
 DAH_DT_Vertical hdc c
 DAH_DT_Glyphs hdc c

 DrawShadowText(hdc L"CALL DrawShadowText" 19 +&c 0 0xff0000 0xff00 7 7)
 c.Init(hdc 10 10 300 30); DAH_DrawText hdc c; c.Init(hdc 8 8 300 30); DAH_DrawText hdc c ;;create shadow text

RestoreDC(hdc sdc)

 note:
 If this is buffered paint, if we would not restore DC after SetViewportExtEx/SetMapMode/SetWorldTransform,
 BitBlt would resize etc. Our text-capture dll does not handle it. Anyway, nobody will do it, eg because of bad quality.
