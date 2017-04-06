function# str&s [flags] ;;flags: 1 don't wait, 2 trim newline

 Gets the console output text of the process started by Exec.
 Returns 1 if successful, 0 if failed (eg the process ended).

 s - variable that receives text.
 flags:
   1 - if new text still not available, don't wait. Then return -1.
   2 - if text ends with new line characters, remove them. Removes single newline, not all.

 REMARKS
 Waits until the console process writes some new text to its standard output or standard error stream, and gets that text.
 The text may have 1 or more lines.


if(!_hProcess) end ERR_INIT
s.all

int r
rep
	if(!PeekNamedPipe(_soRead 0 0 0 &r 0)) ret ;;makes easier to end thread etc
	if(r) break
	if(flags&1) ret -1
	0.01

r+10000
str s1.all(r)
if(!ReadFile(_soRead s1 r &r 0)) ret
s.get(s1.lpstr 0 r)
OemToChar s s ;;convert from OEM character set. Use A because W incorrectly converts newlines etc.
if(_unicode) s.unicode(s 0); s.ansi

if flags&2
	if(s.end("[]")) s.fix(s.len-2); else if(s.end("[10]")) s.fix(s.len-1)

ret 1
