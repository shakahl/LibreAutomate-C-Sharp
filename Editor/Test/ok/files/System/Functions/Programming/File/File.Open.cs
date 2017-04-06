function $_file [$mode] ;;mode: "r" read (default), "w" write new, "a" append, "r+" read/write, "w+" read/write new, "a+" read/append.

 Creates or opens file, and initializes this variable.

 _file - file.
 mode:
  "r" Opens for reading. Error if the file does not exist.
  "w" Opens an empty file for writing. If the given file exists, its contents are destroyed.
  "a" Opens for writing at the end of the file (appending); creates the file first if it doesn't exist.
  "r+" Opens for both reading and writing. The file must exist.
  "w+" Opens an empty file for both reading and writing. If the given file exists, its contents are destroyed.
  "a+" Opens for reading and appending; creates the file first if it doesn't exist.
   These characters can be appended:
     S optimize for sequential access.
     R optimize for random access.
     T optimize for temporary file.
     D delete file when closed.

 REMARKS
 Wraps <google>MSVCRT fopen function</google>.
 Sets binary mode.
 QM 2.3.5. When creating or opening for writing, creates parent folder if does not exist.


str s.expandpath(_file) sm(mode)
if(sm.len) sm+"b"; else sm="rb"
int retry
 g1
if(m_file) fclose(m_file)
m_file=_wfopen(@s @sm)
if(!m_file)
	if(!retry and errno=2 and findc(sm 'r')<0)
		_s.getpath(_file "")
		if _s.len
			retry=mkdir(_s); err
			if(retry) goto g1
	end s.dllerror("cannot open :" "C")
