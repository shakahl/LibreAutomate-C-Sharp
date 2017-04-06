 /
function $s [flags] ;;flags: 1 stderr, 2 no newline, 4 don't redirect

 In exe shows text in console window of this or parent process.

 s - the text.
 flags:
   1 - write to stderr, not stdout.
   2 - don't append newline.
   4 - always write to console, ignore redirection. Better supports Unicode.

 REMARKS
 Writes to stdout or stderr, like C functions printf or fprintf.
 Supports cmd.exe and redirection (eg QM function RunConsole2).
 Exe must be created as console.
 If called not in exe, this function just calls out().

 Example: <open>QM-console</open>.


#if EXE!1
out s
#else

dll- msvcrt #_cputws @*string
dll- msvcrt FILE*__iob_func

if(!s) s=""
if flags&4
	if(flags&2=0) s=_s.from(s "[]")
	_cputws @s
else
	if(_unicode) _s=s; _s.ConvertEncoding(_unicode GetConsoleOutputCP); s=_s
	if flags&1
		FILE* p=__iob_func; int h=&p[2]
		if(flags&2) fprintf h "%s" s; else fputs s h
	else
		if(flags&2) printf "%s" s; else puts s

 note: _cputws uses WriteConsole, it supports Unicode but not redirection. Other API use WriteFile, it supports redirection but not Unicode.
 note: with WriteConsole/WriteFile much work, eg need to do differently when redirected.
