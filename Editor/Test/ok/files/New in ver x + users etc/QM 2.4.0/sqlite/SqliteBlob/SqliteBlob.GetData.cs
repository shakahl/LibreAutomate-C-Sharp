function rowid !*data nBytes [*nbRead]

int retry
 g1
if(m_b and __sqlite.sqlite3_blob_reopen(m_b rowid)) Delete
if(!m_b and __sqlite.sqlite3_blob_open(m_db.conn m_dbName m_table m_column rowid 0 &m_b)) goto ge

if(nBytes<0) nBytes=__sqlite.sqlite3_blob_bytes(m_b); str* s=data; data=s.all(nBytes 2)
else if(nbRead) int nb=__sqlite.sqlite3_blob_bytes(m_b); nBytes=iif(nb<nBytes nb nBytes); *nbRead=nBytes

if(!__sqlite.sqlite3_blob_read(m_b data nBytes 0)) ret

if(!retry) retry=1; Delete; goto g1
 ge
end __sqlite.sqlite3_errmsg(m_db.conn)
