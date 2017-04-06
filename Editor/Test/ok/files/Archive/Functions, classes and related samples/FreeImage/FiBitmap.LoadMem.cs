function fif_type str&mem [flags]

 Loads image file data from memory (str variable) and sets this variable to manage the FIBITMAP object.
 Calls FreeImage_LoadFromMemory. Passes fif_type and flags unchanged.
 Error if failed.

 REMARKS
 To load from macro resources, you can use Load instaed.


Delete
if(!mem.len) end ERR_BADARG
int m=FIMG.FreeImage_OpenMemory(mem mem.len)
if(!m) end ERR_FAILED
b=FIMG.FreeImage_LoadFromMemory(fif_type +m flags)
FIMG.FreeImage_CloseMemory(+m)
if(!b) end ERR_FAILED
