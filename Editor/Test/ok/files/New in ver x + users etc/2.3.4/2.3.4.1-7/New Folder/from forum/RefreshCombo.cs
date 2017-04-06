 \
function lis [$name]

str- t_file
if(!dir(t_file)) ret
str s sd.getfile(t_file)

SendMessage(lis CB_RESETCONTENT 0 0)
foreach s sd
	if s.beg("[")
		s.get(s 1 s.len-2)
		SendMessage(lis CB_ADDSTRING 0 s)

if(!empty(name)) CB_SelectItem lis CB_FindItem(lis name 0 1)
