 gets all child/descendant processes of explorer

int pidParent=ProcessNameToId("explorer")
if(!pidParent) ret
ARRAY(int) a; a[]=pidParent; int i

__Handle hs=CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS 0)
PROCESSENTRY32 p.dwSize=sizeof(p)
Process32First(hs &p)
rep
	for i 0 a.len
		if p.th32ParentProcessID=a[i]
			out F"pid={p.th32ProcessID}  name={&p.szExeFile%%s}"
			a[]=p.th32ProcessID
	if(!Process32Next(hs &p)) break

for i 0 a.len
	ShutDownProcess a[i] 1
