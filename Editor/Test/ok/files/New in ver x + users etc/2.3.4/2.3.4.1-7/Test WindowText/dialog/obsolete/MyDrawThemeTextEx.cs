 /dlg_apihook
function# hTheme hdc iPartId iStateId @*pszText cchText dwTextFlags RECT*pRect DTTOPTS*pOptions

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnDrawThemeTextEx hTheme hdc iPartId iStateId pszText cchText dwTextFlags pRect pOptions)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(R or dwTextFlags&DT_CALCRECT) ret R

CommonTextFunc 8 hdc pszText cchText pRect

ret R
