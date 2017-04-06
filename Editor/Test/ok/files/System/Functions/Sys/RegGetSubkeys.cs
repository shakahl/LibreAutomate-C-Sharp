 /
function# ARRAY(str)&subkeys $parentkey [hive]

 Gets names of all subkeys of a registry key.
 Returns: 1 success, 0 failed.

 subkeys - variable for names of subkeys.
 parentkey, hive - key and hive, like with <help>rget</help>.

 See also: <RegKey.Open>.


int n i; RegKey k
subkeys.redim
if(!k.Open(parentkey hive KEY_ENUMERATE_SUB_KEYS)) ret
BSTR b.alloc(1000)

for i 0 1000000000
	n=1000
	if(RegEnumKeyExW(k.hkey i b &n 0 0 0 0)) break
	subkeys[].ansi(b)
ret 1
