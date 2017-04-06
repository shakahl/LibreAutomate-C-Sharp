out
ARRAY(str) a
 RegGetSubkeys a "CLSID" HKEY_CLASSES_ROOT
RegGetSubkeys a "CLSID" HKEY_CLASSES_ROOT|HKEY_64BIT
int i
for i 0 a.len
	out a[i]
	