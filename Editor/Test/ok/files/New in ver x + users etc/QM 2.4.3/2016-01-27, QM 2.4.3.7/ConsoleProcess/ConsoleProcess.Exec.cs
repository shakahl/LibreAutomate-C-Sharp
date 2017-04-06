function# $cl [$curDir] [flags] ;;flags: 1 show window

 Starts a console process.
 Error if fails or the file does not exist.

 cl - program name or full path, optionally followed by command line parameters.
   Must be exe, not a document.
   Example: "$desktop$\folder\program.exe /a ''c:\new folder\file.txt''".
   Program path should be enclosed in quotes if contains spaces.
   Expands path even if program path is enclosed in quotes. Example: "''$my qm$\an.exe'' /a".
 curDir - current directory for the program.
 flags:
   0-255 - console window show state, like with ShowWindow. If 0 (default), it is hidden.

 REMARKS
 Unlike <help>RunConsole2</help>, does not wait until the process ends.
 Expands special folder string in program's path and in curDir, but not in command line arguments.
 While waiting, this thread cannot receive window/dialog messages, COM events, hooks. If need, call this function from separate thread (mac).


 clear variable if reusing
_siWrite.Close; _soRead.Close; _hProcess.Close

 create pipes
__Handle hOutWrite hInRead
SECURITY_ATTRIBUTES sa.nLength=sizeof(SECURITY_ATTRIBUTES); sa.bInheritHandle=1

if(!CreatePipe(&_soRead &hOutWrite &sa 0)) end "" 16
SetHandleInformation(_soRead HANDLE_FLAG_INHERIT 0)

if(!CreatePipe(&hInRead &_siWrite &sa 0)) end "" 16
SetHandleInformation(_siWrite HANDLE_FLAG_INHERIT 0)

 create process
PROCESS_INFORMATION pi
STARTUPINFOW si.cb=sizeof(STARTUPINFOW)
si.dwFlags=STARTF_USESTDHANDLES|STARTF_USESHOWWINDOW
si.hStdOutput=hOutWrite
si.hStdError=hOutWrite
si.hStdInput=hInRead
si.wShowWindow=flags&255
str s1 s2 s3
if(cl and cl[0]=34) s1.expandpath(cl+1); s1-"''"; else s1.expandpath(cl)
if(!empty(curDir)) s2.expandpath(curDir)

if(!CreateProcessW(0 @s1 0 0 1 CREATE_NEW_CONSOLE 0 @s2 &si &pi)) end "" 16

hOutWrite.Close
CloseHandle(pi.hThread)
_hProcess=pi.hProcess
