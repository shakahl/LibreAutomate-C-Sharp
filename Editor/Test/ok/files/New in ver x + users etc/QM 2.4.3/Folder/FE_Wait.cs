 /
function# ^period ^timeout [flags] ;;flags: 1 on timeout break loop (no error)

 Use with foreach to wait for a condition.
 In each loop (except the first) waits period seconds.
 Error if still called after timeout seconds.

 Added in: QM 2.4.2.

 EXAMPLE
  wait until file exists and is not empty
 foreach(0.5 60 FE_Wait) if(GetFileOrFolderSize("c:\file.txt")>0) break


double waited
if(!waited) waited=1E-9; ret 1
if waited>timeout
	if(flags&1) ret
	end ERR_TIMEOUT
opt waitmsg -1
wait period
waited+period
ret 1
