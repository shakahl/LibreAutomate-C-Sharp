function'int* $macro $resName

 Copies code and function addresses from this variable to an auto-managed memory.
 Returns pointer to the first address. Returns 0 if unavailable.
 Saves code/memory in macro resource resName.
 Clears this variable.

 macro, resName - same as with <help>_qmfile.ResourceAdd</help>.


lock

int* m=m_code; if(!m) ret
int size(m[0]) nFunc(m[1]) i
for(i 2 nFunc+2) m[i]-m ;;address -> offset
_qmfile.ResourceAdd(macro resName m size)
m=_CopyCode(m)
#opt nowarnings 1
__Clear
ret m
