function# $filename

 Loads ANSI or Unicode text file into this variable, and converts to UTF-8 format.
 Error if file not found or fails to load.

 Returns:
   0 - ANSI or UTF-8 without BOM. The function does not convert it.
   1 - UTF-8 with BOM.
   2 - UTF-16 with BOM.
   3 - UTF-16 big endian with BOM.
   4 - UTF-16 without BOM. Note that sometimes may fail to recognize.

 REMARKS
 UTF-8 is QM text format when QM is running in Unicode mode. In ANSI mode it also is compatible in most cases.
 Unicode files often have several special bytes at the beginning. It is byte order mark (BOM). The function removes it.
 This function does not try to recognize UTF-32 and other encodings.
 Possible error if the file is binary or too big (not enough memory).

 See also: <str.SaveUnicodeFile>
 Added in: QM 2.3.2.


this.getfile(filename)

if(this.len<2) ret
word* w=this

if w[0]=0xBBEF and this[2]=0xBF ;;UTF-8
	this.remove(0 3)
	ret 1

if(this.len&1) ret
int r
sel w[0]
	case 0xFEFF ;;UTF-16 LE
	w+2; r=2
	
	case 0xFFFE ;;UTF-16 BE
	w+2; r=3
	_swab +w +w this.len-2
	
	case else ;;simple UTF-16 test. Could use IsTextUnicode, but it is not so fast.
	if(!memchr(this 0 this.len)) ret
	r=4

_s.ansi(w CP_UTF8); this.swap(_s)
ret r

err+ end _error
