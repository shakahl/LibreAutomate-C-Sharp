ClearOutput
str s

 foreach s "Software" FE_RegKey 0 1
	 out s

 foreach s "\Tools" FE_RegKey
	 out s

ARRAY(str) a; int i
 RegGetSubkeys a "Software"
 for i 0 a.len
	 out a[i]

 RegGetValues a "\Tools"
 for i 0 a.len
	 out a[i]

 RegGetValues a "\Tools" 0 1
 for i 0 a.len
	 out a[0 i]
	 out a[1 i]
