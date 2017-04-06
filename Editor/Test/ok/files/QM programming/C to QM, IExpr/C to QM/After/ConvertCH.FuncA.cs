 /CtoQM

 Changes function names from FuncA to Func, except if FuncW does not exist.

str k v rr
m_mf.EnumBegin
rep
	if(!m_mf.EnumNext(k v)) break
	if(k[k.len-1]!='A') continue
	str sw.fromn(k k.len-1 "W" 1)
	if(!m_mf.Get(sw))
		 out k
		continue
	sw.fix(sw.len-1)
	
	 v.findreplace(k sw 6 "" 6)
	rr.format("$1[%s]$2" k)
	v.replacerx("^(dll \S+ )(\S+)A(?= |$)" rr 4)
	 out v
	 It is better to declare [FuncA]Func instead of just Func, because for some FuncA functions also are Func functions in dll (possibly invalid). Also, loads faster, and looks more professional.
	
	if(!AddToMap(m_mf sw v)) continue
	m_mf.Remove(k)
	 out k
	
	lpstr comm=m_mcomm.Get(k)
	if(comm)
		m_mcomm.Add(sw comm); err
		m_mcomm.Remove(k)

 This function should be not necessary because now IsDefAlias does its work. But after removing it something was wrong, so I leaved it instead of debugging.

 Bug: Some interface functions are with A. For example, if there is #define GetObject GetObjectA, all interface functions named GetObject are GetObjectA. Don't fix fbc.
