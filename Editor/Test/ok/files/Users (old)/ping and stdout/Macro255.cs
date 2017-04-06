def GENERIC_READ     0x80000000
def GENERIC_WRITE    0x40000000
def FILE_SHARE_READ  0x1
def FILE_SHARE_WRITE  0x2
def OPEN_EXISTING    3
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

int hStdOutput=CreateFile("CONOUT$" GENERIC_READ|GENERIC_WRITE FILE_SHARE_READ|FILE_SHARE_WRITE 0 OPEN_EXISTING 0 0)
out _s.dllerror
out hStdOutput
str s.all(5000)
SetFilePointer hStdOutput 0 0 0
_i=5
out ReadFile(hStdOutput s 5 &_i 0)
out _s.dllerror
s.fix(_i)
out s
CloseHandle hStdOutput

