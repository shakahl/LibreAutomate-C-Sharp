 /
function# cbFunc cbParam $cl [$curDir] [flags] ;;flags: 1 show window

 Runs a console program and calls a callback function to get its output in real time.
 Returns the exit code of the process.
 Error if fails or the file does not exist.

 cbFunc, cbParam - callback function, and some value to pass to it. Must begin with:
   function# cbParam $s
    cbParam - cbParam of RunConsoleCallback. Can be declared as TYPE&cbParam if you want to pass address of a variable of type TYPE.
    s - console output line text.
    The return value currently is not used.
 cl - program name or full path, optionally followed by command line parameters.
   Must be exe, not a document.
   Example: "$desktop$\folder\program.exe /a ''c:\new folder\file.txt''".
   Program path should be enclosed in quotes if contains spaces.
   Expands path even if program path is enclosed in quotes. Example: "''$my qm$\an.exe'' /a".
 curDir - current directory for the program.
 flags:
   0-255 - console window show state, like with ShowWindow. If 0 (default), it is hidden.

 REMARKS
 Unlike <help>RunConsole2</help>, allows to capture console output lines immediately, not when the process ends.
 Expands special folder string in program's path and in curDir, but not in command line arguments.
 While waiting, this thread cannot receive window/dialog messages, COM events, hooks. If need, call this function from separate thread (mac).

 EXAMPLE
 out
 int ec=RunConsoleCallback(&sub.OnConsoleOutput 0 "$my qm$\console2.exe /ab cd")
 out ec
 
 #sub OnConsoleOutput
 function# cbParam $s
 out F"<{s}>"


 create pipe
__Handle hProcess hOutRead hOutWrite
SECURITY_ATTRIBUTES sa.nLength=sizeof(SECURITY_ATTRIBUTES); sa.bInheritHandle=1

if(!CreatePipe(&hOutRead &hOutWrite &sa 0)) end "" 16
SetHandleInformation(hOutRead HANDLE_FLAG_INHERIT 0)

 create process
PROCESS_INFORMATION pi
STARTUPINFOW si.cb=sizeof(STARTUPINFOW)
si.dwFlags=STARTF_USESTDHANDLES|STARTF_USESHOWWINDOW
si.hStdOutput=hOutWrite
si.hStdError=hOutWrite
si.wShowWindow=flags&255
str s1 s2 s3
if(cl and cl[0]=34) s1.expandpath(cl+1); s1-"''"; else s1.expandpath(cl)
if(!empty(curDir)) s2.expandpath(curDir)

if(!CreateProcessW(0 @s1 0 0 1 CREATE_NEW_CONSOLE 0 @s2 &si &pi)) end "" 16

hOutWrite.Close
CloseHandle(pi.hThread)
hProcess=pi.hProcess

 read pipe
s1.all(10000)
int r
rep
	if(!PeekNamedPipe(hOutRead 0 0 0 &r 0)) break ;;makes easier to end thread etc
	if(r<10000) 0.01
	if(!r) continue
	if(!ReadFile(hOutRead s1 10000 &r 0)) break
	s2.get(s1.lpstr 0 r)
	OemToChar s2 s2 ;;convert from OEM character set. Use A because W incorrectly converts newlines etc.
	if(_unicode) s2.unicode(s2 0); s2.ansi
	
	foreach s3 s2
		call(cbFunc cbParam s3)

int ec
if(!GetExitCodeProcess(hProcess &ec)) ec=-1000

ret ec
