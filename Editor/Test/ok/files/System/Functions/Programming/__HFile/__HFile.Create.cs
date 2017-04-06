function# $fileName creation [access] [shareMode] [flagsAndAttr] ;;creation: OPEN_EXISTING, OPEN_ALWAYS, CREATE_ALWAYS, CREATE_NEW, TRUNCATE_EXISTING;   access: GENERIC_READ, GENERIC_WRITE;   shareMode: FILE_SHARE_READ, FILE_SHARE_WRITE 

 Creates or opens file, and initializes this variable.
 Returns file handle. Error if fails.

 fileName - full path of the file.
 creation - must be one of the above creation constants.
 access - one or more of the above access constants. If 0, uses GENERIC_READ|GENERIC_WRITE.
 shareMode - 0 or one or more of the above shareMode constants. Default is 0 (don't share).
 flagsAndAttr - see CreateFile documentation in the MSDN Library. If 0, uses FILE_ATTRIBUTE_NORMAL.

 REMARKS
 Wraps <google>CreateFile</google>, the main Windows API file create/open function.
 QM 2.3.5. When creating or opening for writing, creates parent folder if does not exist.


if(handle) Close
if(!access) access=GENERIC_READ|GENERIC_WRITE
if(!flagsAndAttr) flagsAndAttr=FILE_ATTRIBUTE_NORMAL

handle=CreateFileU(fileName access shareMode 0 creation flagsAndAttr 0)
if(handle=-1) handle=0; end F"{ERR_FILECREATE}: {fileName}" 16
ret handle
