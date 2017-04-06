function $server $topic $item $data [datalen] [^timeout] ;;for strings, datalen should be omitted or <=0

 Connects and sends a string.


Connect(server topic)
if(!Poke2(item data datalen timeout)) end "Dde.Poke failed: 0x%X" 0 DdeGetLastError(m_idinst)
Disconnect
err+ end _error
