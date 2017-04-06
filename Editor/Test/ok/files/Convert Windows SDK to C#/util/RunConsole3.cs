 /
function# $cl [str&sout] [str&serr] [$curDir] [flags] ;;flags: 1 show window, 0x100 correct newlines

 Runs a console program, waits and captures its output.
 Returns the exit code of the process.
 Error if fails or the file does not exist.

 cl - program name or full path, optionally followed by command line parameters.
   Must be exe, not a document.
   Example: "$desktop$\folder\program.exe /a ''c:\new folder\file.txt''".
   Program path should be enclosed in quotes if contains spaces.
   QM 2.4.2. Expands path even if program path is enclosed in quotes. Example: "''$my qm$\an.exe'' /a".
 sout  - str variable that receives standard output. If omitted or 0, displays in QM output pane.
 sout  - str variable that receives standard error output. If omitted or 0, displays in QM output pane.
 curDir - current directory for the program.
 flags:
   0-255 - console window show state, like with ShowWindow. If 0 (default), it is hidden.
   0x100 (QM 2.3.5) - replace nonstandard newlines in output. For example, replaces "line1[10]line2[13]" with "line1[]line2[]".

 REMARKS
 Expands special folder string in program's path and in curDir, but not in command line arguments.
 While waiting, this thread cannot receive window/dialog messages, COM events, hooks. If need, call this function from separate thread (mac).

 If current process is a QM console exe and you want the child process to use its console (like cmd.exe does), instead use <help>_spawnl</help> or similar function, or <help>CreateProcess</help>. See example.

 Added in: QM 2.3.0.

 EXAMPLES
 if(RunConsole2("schtasks.exe /?")) out "failed"
 
  run console from console exe
 int ec=_spawnl(0 _s.expandpath("$system$\ipconfig.exe") "ipconfig" "/?" 0); if(ec=-1) end _s.dllerror("" "C")


str sout2; if(&sout) sout.all; else &sout=sout2
str serr2; if(&serr) serr.all; else &serr=serr2

 create pipe
__Handle hProcess hOutRead hOutWrite hErrRead hErrWrite
SECURITY_ATTRIBUTES sa.nLength=sizeof(SECURITY_ATTRIBUTES); sa.bInheritHandle=1

if(!CreatePipe(&hOutRead &hOutWrite &sa 0)) end "" 16
SetHandleInformation(hOutRead HANDLE_FLAG_INHERIT 0)
if(!CreatePipe(&hErrRead &hErrWrite &sa 0)) end "" 16
SetHandleInformation(hErrRead HANDLE_FLAG_INHERIT 0)

 create process
PROCESS_INFORMATION pi
STARTUPINFOW si.cb=sizeof(STARTUPINFOW)
si.dwFlags=STARTF_USESTDHANDLES|STARTF_USESHOWWINDOW
si.hStdOutput=hOutWrite
si.hStdError=hErrWrite
si.wShowWindow=flags&255
str s1 s2
if(cl and cl[0]=34) s1.expandpath(cl+1); s1-"''"; else s1.expandpath(cl)
if(!empty(curDir)) s2.expandpath(curDir)

if(!CreateProcessW(0 @s1 0 0 1 CREATE_NEW_CONSOLE 0 @s2 &si &pi)) end "" 16

hOutWrite.Close
hErrWrite.Close
CloseHandle(pi.hThread)
hProcess=pi.hProcess

 read pipe
s1.all(10000)
int r
rep
	if(!PeekNamedPipe(hOutRead 0 0 0 &r 0)) break ;;makes easier to end thread etc
	if(r<10000) 0.01
	if r
		if(!ReadFile(hOutRead s1 10000 &r 0)) break
		sout.geta(s1.lpstr 0 r)
	if(!PeekNamedPipe(hErrRead 0 0 0 &r 0)) break
	if(r<10000) 0.01
	if r
		if(!ReadFile(hErrRead s1 10000 &r 0)) break
		serr.geta(s1.lpstr 0 r)

wait 0 H hProcess
int ec
if(!GetExitCodeProcess(hProcess &ec)) ec=-1000

if sout.len
	OemToChar sout sout ;;convert from OEM character set. Use A because W incorrectly converts newlines etc.
	if(_unicode) sout.unicode(sout 0); sout.ansi
	if(flags&0x100) sout.replacerx("\r(?!\n)" "[]"); sout.replacerx("(?<!\r)\n" "[]") ;;single \r or \n to \r\n
	if(&sout=&sout2) out sout

if serr.len
	OemToChar serr serr ;;convert from OEM character set. Use A because W incorrectly converts newlines etc.
	if(_unicode) serr.unicode(serr 0); serr.ansi
	if(flags&0x100) serr.replacerx("\r(?!\n)" "[]"); serr.replacerx("(?<!\r)\n" "[]") ;;single \r or \n to \r\n
	if(&serr=&serr2) out F"ERROR OUTPUT:[]{serr}"

ret ec
