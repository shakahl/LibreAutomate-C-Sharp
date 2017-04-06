out
typelib Scripting {420B2830-E718-11CF-893D-00A0C9054228} 1.0

Scripting.Dictionary m._create

str file1=
 aaa, 123, aaa
 bbb, 234, bbb

str file2=
 ccc, 234, ccc
 ddd, 345, ddd

VARIANT vk vv
str line number s
ARRAY(str) a

foreach line file1
	if(number.gett(line 1 ",")<0) continue
	number.trim
	 out number
	vk=number; vv=line
	m.Add(vk vv)

foreach line file2
	if(number.gett(line 1 ",")<0) continue
	number.trim
	 out number
	vk=number
	if(m.Exists(vk))
		s=m.Item(vk)
		out s ;;found line from file1
		 ...
	 else
		  ...
