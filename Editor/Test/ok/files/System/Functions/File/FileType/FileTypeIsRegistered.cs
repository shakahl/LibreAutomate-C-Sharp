 /
function! $ext

 Returns 1 if the file type is registered, 0 if not.

 ext - filename extension, eg "vig".

 Added in: QM 2.3.4.


ret sub_sys.FileType_GetClass(ext _s)
