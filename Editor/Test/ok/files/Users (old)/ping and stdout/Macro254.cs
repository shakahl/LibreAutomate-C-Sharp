def STD_OUTPUT_HANDLE  -11
def STARTF_USESTDHANDLES  0x100
type STARTUPINFO cb $lpReserved $lpDesktop $lpTitle dwX dwY dwXSize dwYSize dwXCountChars dwYCountChars dwFillAttribute dwFlags @wShowWindow @cbReserved2 lpReserved2 hStdInput hStdOutput hStdError
type PROCESS_INFORMATION hProcess hThread dwProcessId dwThreadId
type OVERLAPPED Internal InternalHigh offset OffsetHigh hEvent
dll kernel32 #CreateProcess $lpApplicationName $lpCommandLine SECURITY_ATTRIBUTES*lpProcessAttributes SECURITY_ATTRIBUTES*lpThreadAttributes bInheritHandles dwCreationFlags !*lpEnvironment $lpCurrentDriectory STARTUPINFO*lpStartupInfo PROCESS_INFORMATION*lpProcessInformation
dll kernel32 #GetStdHandle nStdHandle
dll kernel32 #ReadFile hFile !*lpBuffer nNumberOfBytesToRead *lpNumberOfBytesRead OVERLAPPED*lpOverlapped
dll kernel32 #SetFilePointer hFile lDistanceToMove *lpDistanceToMoveHigh dwMoveMethod

lpstr cl="ping -n 3 -w 1000 www.quickmacros.com"

STARTUPINFO si.cb=sizeof(STARTUPINFO)
si.hStdOutput=GetStdHandle(STD_OUTPUT_HANDLE)
si.dwFlags=STARTF_USESTDHANDLES
PROCESS_INFORMATION pi
if(!CreateProcess(0 cl 0 0 1 0 0 0 &si &pi)) ret
wait 0 H pi.hProcess
str s.all(5000)
SetFilePointer si.hStdOutput 0 0 0
_i=5
out ReadFile(si.hStdOutput s 5 &_i 0)
out _s.dllerror
s.fix(_i)
out s
CloseHandle pi.hProcess
CloseHandle pi.hThread
