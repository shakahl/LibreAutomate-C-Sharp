 /DEX_Main3
function hDlg htv

DEX_DATA& d=+DT_GetParam(hDlg)

SetWinStyle htv TVS_HASBUTTONS|TVS_HASLINES|TVS_LINESATROOT 1

WINAPI.TreeView_SetImageList(htv d.il 0)

int hif
int first=DEX_TvAdd2(htv 0 "One" 2001 4)
hif=DEX_TvAdd2(htv 0 "Folder" 0 9)
DEX_TvAdd2 htv hif "Two" 2002 5
DEX_TvAdd2 htv 0 "Three" 2003 6
