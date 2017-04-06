
 Writes file buffer to disk.

 REMARKS
 For better performance, Write() and other functions don't write to disk immediately. OS writes to disk later. You can optionally use this function to write to disk now.


if(!m_file) end ERR_INIT

fflush(m_file)
