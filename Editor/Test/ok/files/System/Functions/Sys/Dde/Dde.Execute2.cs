function# $data [^timeout]

 Executes a command.


if(!m_conv) end ERR_INIT
if(!data) end ERR_BADARG
data=_s.unicode(data)
if(timeout<=0) timeout=60
ret DdeClientTransaction(data _s.len+2 m_conv 0 0 XTYP_EXECUTE timeout*1000 0)
