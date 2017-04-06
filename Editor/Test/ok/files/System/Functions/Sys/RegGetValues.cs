 /
function# ARRAY(str)&values $parentkey [hive] [getdata]

 Gets all values in a registry key.
 Returns: 1 success, 0 failed.

 values - variable for values.
   If getdata is nonzero, also gets data. Then array will have 2 dimensions. First dimension has 2 elements - for value name and for data.
 parentkey, hive - key and hive, like with <help>rget</help>.

 See also: <RegKey.Open>.


int nv i j; RegKey k
values.redim
if(!k.Open(parentkey hive KEY_QUERY_VALUE)) ret
BSTR b.alloc(1000)
if(getdata&1) values.create(2 0)

for i 0 100000000
	nv=1000
	if(RegEnumValueW(k.hkey i b &nv 0 0 0 0)) break
	if(getdata&1)
		j=values.redim(-1)
		values[0 j].ansi(b)
		rget values[1 j] values[0 j] k
	else values[].ansi(b)

ret 1
