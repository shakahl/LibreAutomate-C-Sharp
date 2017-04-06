 /
function# resId $tempFile [resType]

 Saves an exe resource to a file.
 Returns 1 on success, 0 on failure.

 idRes - resource id or name.
 tempFile - file.
 resType - resource type. Default or 0: RT_RCDATA (10).

 EXAMPLE
 str sf="$temp$\temp.txt"
 if(!ExeResourceToFile(100 sf)) ret
 run sf


str s; int n
byte* b=ExeLoadDataResource(resId n resType)
if(!b) ret
s.fromn(b n)
s.setfile(tempFile); err ret
ret 1
