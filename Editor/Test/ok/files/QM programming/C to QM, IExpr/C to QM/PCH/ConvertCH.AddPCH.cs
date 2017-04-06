 /CtoQM
function $pch_file

str sf s; int i j m
IStringMap* a=&m_mf

 int t1=perf

sf.getfile(pch_file)
for i 0 1000000000
	j=s.getl(sf -i); if(j<0) break
	if(!s.len or s.beg(" ")) continue
	
	if(s.beg("."))
		m=val(s+1)
		if(m<0 or m>=9) end "bad file format" 1
		continue
	
	if(m=4 and s.beg("[9]")) continue ;;including whole body of interface is not necessary and is slow
		
	 out s
	j=findc(s 32); if(j<0) end "bad file format" 1
	s[j]=0
	a[m].Add(s s+j+1); err
	m_mpch.Add(s "")

 int t2=perf
 out t2-t1/1000
