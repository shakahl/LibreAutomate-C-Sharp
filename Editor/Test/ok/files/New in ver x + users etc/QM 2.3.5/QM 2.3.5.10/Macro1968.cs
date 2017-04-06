str get_date=
 c=created
 a=accessed
 m=modified
IStringMap m._create; m.AddList(get_date "=")

ARRAY(str) arr_dates="c[]a[]m"

str string_dates
string_dates=F"{m.Get(arr_dates[0])};{m.Get(arr_dates[1])};{m.Get(arr_dates[2])}"
out string_dates
