 /
function# str&value $regkey [hive] [flags] ;;flags: 1 get names of subkeys instead of values

 Gets values or subkeys in a registry key. Use with foreach.
 See also: <RegKey.Open>.

 EXAMPLE
 str s
 foreach s "Software" FE_RegKey 0 1
	 out s


int i n; BSTR b
RegKey k

if !i
	if(!k.Open(regkey hive iif(flags&1 KEY_ENUMERATE_SUB_KEYS KEY_QUERY_VALUE))) ret
	b.alloc(1000)

n=1000
if(flags&1) if(RegEnumKeyExW(k.hkey i b &n 0 0 0 0)) ret
else if(RegEnumValueW(k.hkey i b &n 0 0 0 0)) ret

value.ansi(b)
i+1
ret 1
