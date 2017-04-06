function $server $topic $data [^timeout]

 Connects and executes a command.


Connect(server topic)
if(!Execute2(data timeout)) end "Dde.Execute failed: 0x%X" 0 DdeGetLastError(m_idinst)
Disconnect
err+ end _error
