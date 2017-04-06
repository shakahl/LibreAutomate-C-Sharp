function $dbFile [flags] ;;flags: 1 out debug info, 2 at first drop today's table

m_flags=flags
if(m_flags&1) out ;;clear output

OpenFile(dbFile)
if(m_flags&2) DropTodaysTable

rep ;;repeat every 1 second
	1
	int h=win ;;active window
	str s.getwinexe(h); err continue ;;program name
	if m_s.len
		m_ns+1
		if(s=m_s) continue ;;don't write while active program did not change
		Write
		m_ns=0
	m_s=s

 destructor writes the remaining part

err+ end _error
