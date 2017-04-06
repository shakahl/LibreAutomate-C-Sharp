 /
function $s [flags] ;;flags: 1 stderr, 2 no newline

 In exe shows text in console window of this or parent process.

 s - the text.

 REMARKS
 Writes to the standard output buffer or error buffer (flag 1).
 Exe must be created as console. For it you need function MakeExe_Console. Put its name in 'Make exe' dialog, 'After' field.
 If exe runs from cmd.exe, can use its console. Else exe creates own console window when starting.
 If called in QM (not in exe), this function just calls out().

 Example: <open>QM-console</open>.


#if !EXE
out s
#else
int+ g_stdout g_stderr
if !g_stdout
	g_stdout=GetStdHandle(STD_OUTPUT_HANDLE)
	g_stderr=GetStdHandle(STD_ERROR_HANDLE)

int h=iif(flags&1 g_stderr g_stdout)
if(flags&2=0) s=_s.from(s "[]")

_s.unicode(s) ;;also need to set console font that supports Unicode characters, eg Lucida Console
out WriteConsoleW(h _s _s.len/2 &_i 0)
