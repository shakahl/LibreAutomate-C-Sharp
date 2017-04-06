 /
function $folder str&s

IStringMap m=CreateStringMap(1|2)
EnumQmFolder folder 1|8 &GQFWL_Enum &m
m.GetList(s)
s.findreplace("[]" " ")
s.wrap(1000)
