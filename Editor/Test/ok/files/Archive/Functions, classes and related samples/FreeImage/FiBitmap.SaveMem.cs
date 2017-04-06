function fif_type str&mem [flags]

 Saves image file data to memory (stores in str variable).
 Calls FreeImage_SaveToMemory. Passes fif_type and flags unchanged.
 Error if failed.


int m=FIMG.FreeImage_OpenMemory(0 0)
int R=FIMG.FreeImage_SaveToMemory(fif_type b +m flags)
if R
	byte* am; int nb
	if(FIMG.FreeImage_AcquireMemory(+m &am &nb)) mem.fromn(am nb)
	else R=0
FIMG.FreeImage_CloseMemory(+m)
if(!R) end ERR_FAILED
