function $server $topic $item str&data [^timeout]

 Connects and retrieves a string.
 Stores it into str variable data.
 Note: Not all programs support Unicode.


Connect(server topic)
if(!Request2(item data timeout)) end "Dde.Request failed: 0x%X" 0 DdeGetLastError(m_idinst)
Disconnect
err+ end _error
