 /
type __SM_CATKEYS perfCounter long'perfArr[11]
function'__SM_CATKEYS*

__SharedMemory+ g_smCatkeys
if(g_smCatkeys.mem=0) g_smCatkeys.Create("Catkeys_SM_0x10000" 0x10000)
ret +g_smCatkeys.mem
