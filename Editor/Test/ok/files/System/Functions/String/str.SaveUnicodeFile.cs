function $filename format ;;format: 0 don't change, 1 UTF-8 BOM, 2 UTF-16 BOM, 3 UTF-16 BE BOM, 4 UTF-16 no BOM

 Saves data of this variable to file in specified Unicode format.

 format - return value of <help>str.LoadUnicodeFile</help>.

 REMARKS
 Data of this variable should be UTF-8. It is QM text format when QM is running in Unicode mode. If format is 0, also can be ANSI.

 Added in: QM 2.3.2.

 
str ss; str& s=ss
sel format
	case 0
	&s=this
	
	case 1
	s.from("[0xef][0xbb][0xbf]" this)
	
	case 2
	s.unicode(this CP_UTF8)
	s-"[0xff][0xfe]"
	
	case 3
	s.unicode(this CP_UTF8)
	_swab s s s.len
	s-"[0xfe][0xff]"
	
	case 4
	s.unicode(this CP_UTF8)

s.setfile(filename)
err+ end _error
