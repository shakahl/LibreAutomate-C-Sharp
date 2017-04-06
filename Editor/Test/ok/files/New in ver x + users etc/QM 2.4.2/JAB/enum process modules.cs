out
 int w=win("Untitled 1 - OpenOffice Writer" "SALFRAME")
int w=win("Untitled 1 - LibreOffice Writer" "SALFRAME")
 int w=win("Stylepad" "SunAwtFrame")
 int w=win("jEdit - Untitled-1" "SunAwtFrame")
PF
str s
int pid; GetWindowThreadProcessId(w &pid)
__Handle hsnap=CreateToolhelp32Snapshot(TH32CS_SNAPMODULE pid)
MODULEENTRY32 m.dwSize=sizeof(m)
int ok=Module32First(hsnap &m)
rep
	if(!ok) break
	_s.fromn(&m.szExePath -1); if(_s.begi("C:\Windows")) goto g1
	 s.formata("%s[]" &m.szModule)
	s.formata("%s[]" &m.szExePath)
	 g1
	ok=Module32Next(hsnap &m)
PN;PO
out s

#ret
 svt.dll
 jvmfwk3.dll
 svl.dll
 javaloader.uno.dll
 jvmaccess3MSC.dll
 javavm.uno.dll
 jvm.dll
