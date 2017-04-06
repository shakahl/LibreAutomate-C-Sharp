 execute this before starting all, eg when QML file loaded
dll kernel32 #WaitForSingleObject hHandle dwMilliseconds
dll kernel32 #ReleaseMutex hMutex
int+ g_mutex1
if(!g_mutex1) g_mutex1=CreateMutex(0 0 0)

 execute this when finished, eg before unloading QML file
 CloseHandle g_mutex1; g_mutex1=0
