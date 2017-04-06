 /dlg_apihook
function# hTheme hdc iPartId iStateId @*pszText cchText dwTextFlags dwTextFlags2 RECT*pRect

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnDrawThemeText hTheme hdc iPartId iStateId pszText cchText dwTextFlags dwTextFlags2 pRect)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(R) ret R

CommonTextFunc 7 hdc pszText cchText pRect

ret R
