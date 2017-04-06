function $line

 Writes string to the file, and appends new line characters.

 REMARKS
 Writes starting from current file pointer, and updates it.


if(!m_file) end ERR_INIT

int r
if(line) r=fputs(line m_file)
if(r>=0) r=fputs("[]" m_file)
if(r<0) end ERR_FAILED
