SetPrivilege "SeDebugPrivilege"

int hs=CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0); if(hs==-1) ret
PROCESSENTRY32 pe.dwSize=sizeof(PROCESSENTRY32)
int k=Process32First(hs, &pe)
rep
	if(!k) break
	if(pe.th32ProcessID)
		out GetProcessCpuUsage(pe.th32ProcessID 0 1)
		err out "%s" &pe.szExeFile
	 int ph = OpenProcess(1, 0, pe.th32ProcessID)
	 out ph
	 if(ph) CloseHandle(ph)
	k=Process32Next(hs, &pe)
	
CloseHandle(hs)
