int w=win("Form" "#32770")
act w
int c=child("" "SysHeader32" w)
 act c
PostMessage c WM_SETFOCUS 0 0
PostMessage c WM_KEYDOWN VK_RIGHT 0
PostMessage c WM_KEYDOWN VK_SPACE 0
PostMessage c WM_KILLFOCUS 0 0




 int c=child("FolderView" "SysListView32" w 0x0 "id=1") ;;list 'Folder View'
 Acc ach.Find(c "COLUMNHEADER" "Date modified" "class=SysHeader32" 0x1015)
  ach.DoDefaultAction
 POINT p
 ach.Location(p.x p.y)
 ScreenToClient c &p; int xy=MakeInt(p.x+2 p.y+2)
 out "%i %i" p.x p.y
 PostMessage c WM_LBUTTONDOWN MK_LBUTTON xy
 PostMessage c WM_LBUTTONDOWN 0 xy
