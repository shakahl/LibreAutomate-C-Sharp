 /
function $s [flags] ;;flags: 1 stderr, 2 don't redirect, 4 no newline

 In exe shows text in the console window of this or parent process.

 s - the text.
 flags:
   1 - write to the standard error output.
   2 - always write to console, ignore redirection. Better supports Unicode.
   4 - don't append newline.

 REMARKS
 Writes to the standard output (usually console window), like C function printf.
 In console exe supports cmd.exe and console output redirection (eg QM function RunConsole2).
 If used not in exe, calls <help>out</help> instead.
 If fails, calls <help>mes</help>.
 If there is no console, auto-creates (AllocConsole). Then does not support cmd.exe. To create console exe, check 'Console' in 'Make exe' dialog. <help #IDH_MAKEEXE>Read more</help>.
 Partially supports Unicode. The console should use font that supports Unicode characters, eg Lucida Console.
 Tip: You can use Windows console API to get/change console properties etc, eg <help>SetConsoleMode</help>, <help>SetConsoleTitle</help>.

 Added in: QM 2.4.1.
 See also: <ExeConsoleRedirectQmOutput>, <ExeConsoleRead>.

 EXAMPLE
 ExeConsoleWrite "Type something and press Enter"
 str s
 ExeConsoleRead s
 ExeConsoleWrite F"s=''{s}''"
 2


#if !EXE
out s
#else
opt nowarningshere 1
__ExeConsoleInit

lpstr s0=s
if(flags&4=0) s=_s.from(s "[]")
if(!s) s=""

if flags&2
	_s.unicode(s)
	if(!WriteConsoleW(___outc.conout _s _s.len/2 &_i 0)) mes s0
else
	if(_unicode) _s=s; _s.ConvertEncoding(_unicode GetConsoleOutputCP); s=_s
	int h=iif(flags&1 ___outc.stderr ___outc.stdout)
	if(!WriteFile(h s len(s) &_i 0)) mes s0

 speed: faster than printf etc, sometimes 2 times.
 note: uses mes, not out, because out may be redirected (ExeConsoleRedirectQmOutput).
 note: some API not in WinFunctions. never mind, used only in exe, rarely.
