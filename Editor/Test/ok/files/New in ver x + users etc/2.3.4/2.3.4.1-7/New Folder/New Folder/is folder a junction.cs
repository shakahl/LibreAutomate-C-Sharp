int attr=GetFileAttributesW(@fullPathOfTheFolder); if(attr=-1) end "failed, %s" 0 _s.dllerror
if(attr&FILE_ATTRIBUTE_REPARSE_POINT) out "is junction"
