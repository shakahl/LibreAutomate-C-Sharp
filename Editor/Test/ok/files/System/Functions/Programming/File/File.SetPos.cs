function offset [origin] ;;origin: 0 from beginning, 1 from current, 2 from end.

 Sets file pointer position.

 offset - number of bytes from origin. Can be negative if origin is 1 or 2.

 REMARKS
 If file opened for append, Write() and other functions always move file pointer to the end of file before writing.


if(!m_file) end ERR_INIT

if(fseek(m_file offset origin)) end _s.dllerror("cannot set position: " "C")
