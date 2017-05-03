 /run csc
function# $cl [str&sout] [$curDir] [flags] ;;flags: 1 show window, 0x100 correct newlines

 Runs a console program, waits and captures its output.
 Returns its exit code.
 Error if fails or the file does not exist.

 cl - program name or full path, optionally followed by command line parameters.
   Must be exe or other executable file, not a document.
   Example: "$desktop$\folder\program.exe /a ''c:\new folder\file.txt''".
   Program path should be enclosed in quotes if contains spaces.
   QM 2.4.2. Expands path even if program path is enclosed in quotes. Example: "''$my qm$\an.exe'' /a".
 sout  - str variable that receives the output. If omitted or 0, displays in QM output pane.
 curDir - current directory for the program.
 flags:
   0-255 - console window show state, like with ShowWindow. If 0 (default), it is hidden.
   0x100 (QM 2.3.5) - replace nonstandard newlines in output. For example, replaces "line1[10]line2[13]" to "line1[]line2[]".

 REMARKS
 Expands special folder string in program's path (if not enclosed in "") and in curDir, but not in command line arguments.
 While waiting, this thread cannot receive window/dialog messages, COM events, hooks. If need, call this function from separate thread (mac).

 If current process is a QM console exe and you want the child process to use its console (like cmd.exe does), instead use <help>_spawnl</help> or similar function, or <help>CreateProcess</help>. See example.

 Added in: QM 2.3.0.

 EXAMPLES
 if(RunConsole2("schtasks.exe /?")) out "failed"
 
  run console from console exe
 int ec=_spawnl(0 _s.expandpath("$system$\ipconfig.exe") "ipconfig" "/?" 0); if(ec=-1) end _s.dllerror("" "C")


str sout2; if(&sout) sout.all; else &sout=sout2

 create pipe
__Handle hProcess hOutRead hOutReadTmp hOutWrite hErrWrite
int cp=GetCurrentProcess
SECURITY_ATTRIBUTES sa.nLength=sizeof(SECURITY_ATTRIBUTES); sa.bInheritHandle=1

if(!CreatePipe(&hOutReadTmp &hOutWrite &sa 0)) end "" 16
if(!DuplicateHandle(cp hOutWrite cp &hErrWrite 0 1 DUPLICATE_SAME_ACCESS)) end "" 16
if(!DuplicateHandle(cp hOutReadTmp cp &hOutRead 0 0 DUPLICATE_SAME_ACCESS)) end "" 16
CloseHandle hOutReadTmp; hOutReadTmp=0

 create process
PROCESS_INFORMATION pi
STARTUPINFOW si.cb=sizeof(STARTUPINFOW)
si.dwFlags=STARTF_USESTDHANDLES|STARTF_USESHOWWINDOW|STARTF_FORCEOFFFEEDBACK
si.hStdOutput=hOutWrite
si.hStdError=hErrWrite
si.wShowWindow=flags&255
str s1 s2
if(cl and cl[0]=34) s1.expandpath(cl+1); s1-"''"; else s1.expandpath(cl)
if(!empty(curDir)) s2.expandpath(curDir)
 PN
int cpFlags
if(!GetStdHandle(STD_OUTPUT_HANDLE)) cpFlags|CREATE_NO_WINDOW ;;if console, with this flag would be created conhost process; else this flag makes faster, although conhost process is created regardless of this flag
int suspended=0
if suspended
	if(!CreateProcessW(0 @s1 0 0 1 cpFlags|CREATE_SUSPENDED 0 @s2 &si &pi)) end "" 16
	1
	WakeCPU 100
	PN
	ResumeThread pi.hThread
else
	if(!CreateProcessW(0 @s1 0 0 1 cpFlags 0 @s2 &si &pi)) end "" 16
 CREATE_NO_WINDOW makes faster if called from GUI process. Eg csc.exe then 65 ms (by default 65 or 75, more often 75). Tested all other flags too. But slower if from console process (created conhost process).

CloseHandle hOutWrite; hOutWrite=0
CloseHandle hErrWrite; hErrWrite=0
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
	sout.geta(s1.lpstr 0 r)

int ec
if(!GetExitCodeProcess(hProcess &ec)) ec=-1000

if sout.len
	OemToChar sout sout ;;convert from OEM character set. Use A because W incorrectly converts newlines etc.
	if(_unicode) sout.unicode(sout 0); sout.ansi
	if(flags&0x100) sout.replacerx("\r(?!\n)" "[]"); sout.replacerx("(?<!\r)\n" "[]") ;;single \r or \n to \r\n
	if(&sout=&sout2) out sout

ret ec
