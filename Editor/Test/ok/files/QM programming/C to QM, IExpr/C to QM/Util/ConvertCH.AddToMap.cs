 /CtoQM
function# IStringMap&m $k $v [$comm] [nocomm]

 Adds to map. If already exists and is diferent, appends as comments.
 Also appends comm.

int r=1; str ss; lpstr v2 sc

 if(m_pch) m_mpch.Remove(k)

m.Add(k v)
err
	r=0
	v2=m.Get(k)
	if(!strcmp(v v2)) ret 1
	if(matchw(v "uuidof(*") and !q_stricmp(v v2)) ret 1
		
	 out "%s %s[]%s %s[]----" k v2 k v
	
	if(nocomm) ret
	sc=m_mcomm.Get(k)
	if(sc)
		ss.format("%s[] ;;%s" sc v)
		m_mcomm.Set(k ss)
	else
		ss.format(" ;;%s" v)
		m_mcomm.Add(k ss)
	
if(len(comm))
	m_mcomm.Add(k comm)
	err
		ss.format("%s[]%s" m_mcomm.Get(k) comm)
		m_mcomm.Set(k ss)

ret 1
