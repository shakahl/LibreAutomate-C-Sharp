 /
function $folder ARRAY(int)&a [ARRAY(int)&aLevel]

 Gets QM items in a QM folder and subfolders.

 folder - folder name or path ("\folder1\folder2"). Use "" to include all macros.
 a - variable that receives item ids. Use <help>qmitem</help> or <help>str.getmacro</help> to get item properties.
 aLevel - optional array that receives item levels relative to folder.


a=0
type __GQIF_DATA ARRAY(int)*a ARRAY(int)*aLevel htv
__GQIF_DATA d
d.a=&a
d.aLevel=&aLevel
d.htv=id(2202 _hwndqm)

EnumQmFolder folder 0 &GQIF_Enum &d