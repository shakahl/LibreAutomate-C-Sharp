 /
function $_file attrib [flags] ;;obsolete, use FileSetAttributes.   flags: 0 set, 1 add, 2 remove.

 Sets, adds or removes file attributes.
 Error if fails.

 attrib - FILE_ATTRIBUTE_x flags. <google "site:microsoft.com FILE_ATTRIBUTE_COMPRESSED FILE_ATTRIBUTE_VIRTUAL">Reference</google>.


BSTR sf=_s.expandpath(_file)
int a=GetFileAttributesW(sf); if(a=-1) goto ge
sel flags&3
	case 0
	if(a==attrib) ret 1
	case 1
	if((a&attrib)==attrib) ret 1
	attrib|a
	case 2
	if(!(a&attrib)) ret 1
	attrib=a~attrib
if(!SetFileAttributesW(sf attrib)) goto ge
ret
 ge
end "Cannot set attributes" 16
