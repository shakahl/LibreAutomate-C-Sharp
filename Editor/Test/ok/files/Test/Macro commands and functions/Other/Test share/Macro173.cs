 Get SysListView32 control item text
int hwnd=child("FolderView" "SysListView32" "My Documents" 0x1)
int item=0
LVITEM* lip=share ;;in QM context
LVITEM* lip2=share(hwnd) ;;in hlv context
memset(lip 0 sizeof(LVITEM))
int stringoffset=sizeof(LVITEM)+20
lip.pszText=+(lip2+stringoffset) ;;in hlv context
lip.cchTextMax=260
lip.mask=LVIF_TEXT
SendMessage(hwnd LVM_GETITEMTEXT item lip2)
str s.get(+lip stringoffset)
out s
