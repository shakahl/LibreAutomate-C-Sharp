 /CtoQM
function# str&name str&r

 Replaces dll function names etc. Returns 1 if replaced.
 Called by CalcConst.

if(r.len=name.len+1 and r.beg(name) and r[r.len-1]='A') ;;#define Something SomethingA
	if(m_mf.Get(name)) ret 1
	if(m_mt.Get(name)) ret 1
	if(m_mi.Get(name)) ret 1

int c=r[0]
if(c>='0' and c<='9') ret
if(findrx(r "\W")>=0) ret
if(!r.len) ret

str s rx repl

if(m_mf.Get2(r s))
	 out "%s %s %s" name r s
	rx.format(" (\S*?)%s(?= |$)" r)
	repl.format(" [%s]$1%s" r name)
	s.replacerx(rx repl 4)
	 out "%s %s %s" name r s
	lpstr s2=m_mf.Get(name); if(s2 and !strncmp(s2 "dll ?" 5)) m_mf.Remove(name)
	AddToMap(m_mf name s)
else if(m_mt.Get2(r s))
	 out "%s %s %s" name r s
	s.findreplace(r name 4)
	 out "%s %s %s" name r s
	AddToMap(m_mt name s)
else if(m_mi.Get2(r s))
	 out "%s %s %s" name r s
	s.findreplace(r name 4)
	 out "%s %s %s" name r s
	AddToMap(m_mi name s)
else if(m_mfcb.Get2(r s))
	 out "%s %s %s" name r s
	AddToMap(m_mfcb name s)
else if(m_mtd.Get2(r s))
	 out "%s %s %s" name r s
	AddToMap(m_mtd name s)
	ret 1

 out "%s   %s" name r
if(m_mcomm.Get2(r s)) m_mcomm.Add(name s); err
ret 1
