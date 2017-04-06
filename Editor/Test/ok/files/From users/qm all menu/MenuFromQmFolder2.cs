 /
function $folder str&s [flags] ;;flags: 1 no submenus.
 MenuFromQmFolder
 Gets list of QM items in a QM folder and formats it like QM menu.
 Then you can use the list to show QM menu (DynamicMenu)
 or toolbar (DynamicToolbar). For toolbar, use flag 1.
 Folders named "private" and "System" are excluded.

 folder - folder name or path. Use "" to include all macros.
 s - variable that receives the list. If initially s is not empty, the list is appended.

 EXAMPLES
 str s
 MenuFromQmFolder "Folder" s
 DynamicMenu(s)

 str s=" /siz0 150 700 /ssiz0 100 30 /set0 1|2|4|8|16|128[]"
 MenuFromQmFolder "Folder" s 1
 DynamicToolbar(s "TB from Folder")


type __MF_DATA str*sp str'st str'slt level flags htv
__MF_DATA d.sp=&s
d.flags=flags
d.htv=id(2202 _hwndqm)

EnumQmFolder2 folder 1 &MF_Proc2 &d