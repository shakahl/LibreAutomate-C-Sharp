int h=mac("Toolbar33")
 int h=mac("Toolbar33" win("ownerwindowname" "ownerwindowclass"))
int hc=id(9999 h)
if(_winver>=0x501) SetWindowTheme hc L"" L""
SetWinStyle hc TBSTYLE_FLAT 2
RedrawWindow hc 0 0 RDW_INVALIDATE
