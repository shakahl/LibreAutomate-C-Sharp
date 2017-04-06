function#

 Gets current file pointer position.


if(!m_file) end ERR_INIT

int r=ftell(m_file)
if(r=-1) end _s.dllerror("cannot get position: " "C")
ret r
