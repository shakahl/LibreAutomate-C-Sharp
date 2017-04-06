function#

 Gets current file size (number of bytes).

 Added in: QM 2.3.3.


if(!m_file) end ERR_INIT

int r p
p=ftell(m_file)
fseek(m_file 0 2)
r=ftell(m_file)
fseek(m_file p 0)
ret r
