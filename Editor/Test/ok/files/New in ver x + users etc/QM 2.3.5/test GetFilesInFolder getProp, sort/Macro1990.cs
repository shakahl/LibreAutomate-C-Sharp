out
ARRAY(str) a; int i
GetFilesInFolder a "$my qm$" "" 0 2
a.sort(8)
for i 0 a.len
	DateTime t=val(a[i] 1 _i) ;;use DateTime for times. Use long for size.
	t.UtcToLocal
	str sPath=a[i]+_i+1
	out F"{t.ToStr(4)}    {sPath}"
