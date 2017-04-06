function fif_type $_file [flags]

 Saves to image file.
 Calls FreeImage_Save. Passes fif_type and flags unchanged.
 Error if failed.


if(!FIMG.FreeImage_SaveU(fif_type b @_s.expandpath(_file) flags)) end ERR_FAILED
