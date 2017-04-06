 /dlg_apihook

out "----------------------------"
int n
__Handle hs=CreateToolhelp32Snapshot(TH32CS_SNAPMODULE GetCurrentProcessId)
MODULEENTRY32 m.dwSize=sizeof(m)
int ok=Module32First(hs &m)
rep
	if(!ok) break
	n+1
	lpstr s=&m.szModule
	out s
	ok=Module32Next(hs &m)
out n
