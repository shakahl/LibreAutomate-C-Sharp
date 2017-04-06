\Dialog_Editor

str controls = "4 6 8"
str e4 e6 e8
e8="Copies all items from the old class folder to the new class folder. Does not change text.[][]Old folder name must match class name. Can be path."
if(!ShowDialog("TO_CopyClass" 0 &controls)) ret

e4="\ff\Old"
e6="New"

type CC_DATA ~oldName ~newName ARRAY(POINT)f
CC_DATA d

d.oldName=e4; d.oldName.getfilename
d.newName=e6

POINT& f=d.f[]
f.x=qmitem(e4); if(!f.x) mes- "folder not found"
f.y=newitem(e6 "" "Folder")

EnumQmFolder e4 1|16 &TO_CopyClassProc &d
err mes- _error.description


 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 201 120 "Copy class"
 3 Static 0x54000000 0x0 4 4 56 13 "Old class folder"
 4 Edit 0x54030080 0x200 62 4 136 14 ""
 5 Static 0x54000000 0x0 4 22 56 12 "New class name"
 6 Edit 0x54030080 0x200 62 22 136 14 ""
 8 Edit 0x54230844 0x20000 4 46 194 48 ""
 1 Button 0x54030001 0x4 50 102 48 14 "OK"
 2 Button 0x54030000 0x4 102 102 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""
