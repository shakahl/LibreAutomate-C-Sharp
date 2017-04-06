out
ARRAY(str) a; int i
EnumFilesRX "$system$" "^.*\.exe$" a ;;4|0x80000
for i 0 a.len
	out a[i]
	
 EnumFilesRX "$system$" "^C:\\.*\.exe" a 0x80000
