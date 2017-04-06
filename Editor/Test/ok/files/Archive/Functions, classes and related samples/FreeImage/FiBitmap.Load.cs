function fif_type $_file [flags]

 Loads image file or resource, and sets this variable to manage the FIBITMAP object (auto-delete etc).
 Calls FreeImage_Load. Passes fif_type and flags unchanged.
 Error if failed.


sel _file 2
	case ["resource:*","image:*"]
	_s.getfile(_file)
	if(_file[0]='i') _s.decrypt(32)
	ret LoadMem(fif_type _s flags)
	err+ end _error

Delete
b=FIMG.FreeImage_LoadU(fif_type @_s.expandpath(_file) flags)
if(!b) end ERR_FAILED
