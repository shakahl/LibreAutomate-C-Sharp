 /
function# $_file ;;obsolete, use FileGetAttributes

 Gets file attributes.
 The return value is FILE_ATTRIBUTE_x flags. <google "site:microsoft.com FILE_ATTRIBUTE_COMPRESSED FILE_ATTRIBUTE_VIRTUAL">Reference</google>.
 Error if fails or file does not exist.


int a=GetFileAttributesW(@_s.expandpath(_file))
if(a=-1) end F"{ERR_FAILED} to get file attributes" 16
ret a
