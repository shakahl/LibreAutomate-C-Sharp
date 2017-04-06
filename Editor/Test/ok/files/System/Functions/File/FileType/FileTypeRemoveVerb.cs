 /
function $ext $verb

 Removes a verb (context menu command) from a file type.
 Error if fails or the file type does not exist. Not error if the verb does not exist.

 ext - filename extension, eg "vig". Also can be "*" to remove verb from all file types.
 verb - verb name, eg "Play".

 REMARKS
 QM must be running as administrator.
 If you remove default verb, later use FileTypeSetDefaultVerb to set another default verb.

 Added in: QM 2.3.2.


str cls
if(!sub_sys.FileType_GetClass(ext cls)) end F"{ERR_FAILED}, file type not registered"

rset "" verb _s.from(cls "\Shell") HKEY_CLASSES_ROOT -2
