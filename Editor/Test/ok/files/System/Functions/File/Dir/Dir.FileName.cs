function$ [flags] ;;flags: 1 full, 2 DOS (QM 2.2.0)

 Returns filename or full path.


if(!fd) end ERR_INIT

if(flags&1)
	lpstr s=__e.FullPath
	if(flags&2) str-- _dp; _dp=s; ret _dp.dospath
	ret s
else
	if(flags&2 and fd.cAlternate[0]) ret &fd.cAlternate
	ret &fd.cFileName
