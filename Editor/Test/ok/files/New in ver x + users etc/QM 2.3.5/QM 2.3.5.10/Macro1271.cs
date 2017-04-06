str c="created"
str a="accessed"
str m="modified"


ARRAY(str*) arr_dates
arr_dates.create(3)
arr_dates[0]=&c
arr_dates[1]=&a
arr_dates[2]=&m


str string_dates
string_dates=F"{*arr_dates[0]};{*arr_dates[1]};{*arr_dates[2]}"
out string_dates
