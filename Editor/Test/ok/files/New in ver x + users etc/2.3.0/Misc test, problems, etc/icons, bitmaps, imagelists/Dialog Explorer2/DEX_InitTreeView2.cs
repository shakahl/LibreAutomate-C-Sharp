 /DEX_Main2
function hDlg htv

DEX_DATA2& d=+DT_GetParam(hDlg)

SetWinStyle htv TVS_HASBUTTONS|TVS_HASLINES|TVS_LINESATROOT 1
SendMessage htv TVM_SETIMAGELIST 0 d.il.GetImageList

int hif
int first=TvAdd(htv 0 "One" 2001 3)
hif=TvAdd(htv 0 "Folder" 0 4)
TvAdd htv hif "Two" 2002 5
TvAdd htv 0 "Three" 2003 6

SendMessage htv TVM_SELECTITEM TVGN_CARET first
