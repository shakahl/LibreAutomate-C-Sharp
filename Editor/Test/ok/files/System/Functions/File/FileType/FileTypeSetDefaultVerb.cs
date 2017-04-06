 /
function $ext $verb

 Sets default verb of a file type.
 Error if fails.

 ext - extension, eg "vig".
 verb - verb name, eg "Play".

 REMARKS
 QM must be running as administrator.

 Added in: QM 2.3.2.


str cls
if(!sub_sys.FileType_GetClass(ext cls)) end F"{ERR_FAILED}, file type not registered"

if(!rset(verb "" _s.from(cls "\Shell") HKEY_CLASSES_ROOT)) end ERR_FAILED
