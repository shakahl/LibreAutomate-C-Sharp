function# $path funcaddress [flags] [param] ;;obsolete, use 'Enumerate files' dialog
 flags: 0 file, 1 folder, 2 any, 4 return full path, 8 subfolders

int f=flags&3
if(flags&8) f|4
if(flags&0x10000) f|8
int f2=flags&4/4
int n
foreach this path FE_Dir f
	n=1
	if(!call(funcaddress &this FileName(f2) param)) ret -1
ret n
