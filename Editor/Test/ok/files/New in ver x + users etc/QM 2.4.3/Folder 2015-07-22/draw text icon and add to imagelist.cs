__ImageList il.Create
__MemBmp m.Create(16 16)
RECT r.right=16; r.bottom=16
FillRect m.dc &r COLOR_WINDOW+1
__Font f.Create("Tahoma" 8)
int of=SelectObject(m.dc f)
TextOut m.dc 1 1 "A" 1
SelectObject(m.dc of)
int b=m.Detach
ImageList_Add(il b 0)
DeleteObject b

str s=F",{il}[]one,0"
ShowDropdownListSimple s
