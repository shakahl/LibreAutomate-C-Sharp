function str&s [flags] ;;flags: 1 append

 Writes s to the file.


SetFilePointer handle 0 0 iif(flags&1 FILE_END 0)
if(s.len) if(!WriteFile(handle s s.len &_i 0) or _i!=s.len) end ES_FAILED
if(flags&1=0) SetEndOfFile handle
