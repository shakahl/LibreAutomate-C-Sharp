 /
function $folder str&s [flags] ;;flags: currently not used

 Gets list of QM items in a QM folder and subfolders.
 Folders named "private" and "System" are excluded.

 folder - folder name or path ("\folder1\folder2"). Use "" to include all macros.
 s - variable that receives the list.


type __GQIN_DATA str*sp str'st level flags htv
__GQIN_DATA d.sp=&s
d.flags=flags
d.htv=id(2202 _hwndqm)
s.len=0

EnumQmFolder folder 1 &GQIN_Enum &d