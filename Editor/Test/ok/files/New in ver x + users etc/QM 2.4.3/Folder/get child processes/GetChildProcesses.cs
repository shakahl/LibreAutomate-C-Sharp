 /
function! pidParent ARRAY(int)&aPid [ARRAY(str)&aName]

SetPrivilege "SeDebugPrivilege"
if(&aPid) aPid=0
if(&aName) aName=0
if(!pidParent) pidParent=GetCurrentProcessId
int hs=CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS 0); if(hs==-1) ret
PROCESSENTRY32W pe.dwSize=sizeof(pe)
int k=Process32FirstW(hs &pe)
rep
	if(!k) break
	if pe.th32ParentProcessID=pidParent
		if(&aPid) aPid[]=pe.th32ProcessID
		if(&aName) aName[].ansi(&pe.szExeFile)
	k=Process32NextW(hs &pe)
CloseHandle(hs)
ret 1
