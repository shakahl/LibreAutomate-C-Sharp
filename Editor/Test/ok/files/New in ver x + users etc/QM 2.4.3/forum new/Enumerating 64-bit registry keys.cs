str s

out "32-bit keys:"
foreach s "SOFTWARE" FE_RegKey HKEY_LOCAL_MACHINE 1
	out s

out "64-bit keys:"
foreach s "SOFTWARE" FE_RegKey HKEY_LOCAL_MACHINE|HKEY_64BIT 1
	out s

 Flag HKEY_64BIT also can be used with other QM registry functions.
