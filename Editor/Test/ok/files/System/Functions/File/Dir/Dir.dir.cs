function$ [$pathname] [flags] ;;flags: 0 file, 1 folder, 2 any, 4 return full path.

 Finds file and initializes this variable.
 Returns filename. Returns 0 if not found.

 REMARKS
 Supports <help #IDP_WILDCARD>wildcard characters</help> (*?) in filename part.


opt noerrorshere 1

if !empty(pathname)
	if(!__e) __e=CreateEnumFiles
	if(flags&4) flags|0x100
	__e.Begin(pathname flags&0x103)
else if(!fd) end ERR_NOMOREFILES

lpstr R=__e.Next
if(R) fd=__e.Data; else fd=0
ret R
