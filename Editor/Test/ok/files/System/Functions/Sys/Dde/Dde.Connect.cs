function $server $topic

 Connects to a DDE server.


if(!server or !topic) end ERR_BADARG

___DdeStr hs1 hs2
if(m_conv) DdeDisconnect(m_conv)
m_conv=DdeConnect(m_idinst hs1.Create(server m_idinst) hs2.Create(topic m_idinst) 0)
if(!m_conv) end "DDE server or topic is unavailable"
