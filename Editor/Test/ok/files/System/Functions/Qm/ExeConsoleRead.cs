 /
function str&s

 In exe reads user input text from the console window.

 s - variable that receives the text.

 REMARKS
 Waits until user types or pastes some text and presses Enter.
 While waiting, cannot process Windows messages and COM events. This thread should not have dialogs etc.
 In console exe supports cmd.exe and console input redirection.
 If used not in exe, calls <help>inp</help> instead. Also if fails. In non-console exe auto-creates console.

 Added in: QM 2.4.1.
 See also: <ExeConsoleWrite> (example).


s.all
#if !EXE
inp- s
#else
opt nowarningshere 1
__ExeConsoleInit

str b.all(8000 2)
if(ReadConsoleW(___outc.stdin b b.len/2 &_i 0)) s.ansi(b _unicode _i) ;;supports Unicode, fails if redirected etc
else if(ReadFile(___outc.stdin b b.len &_i 0)) s.left(b _i); if(_unicode) s.ConvertEncoding(GetConsoleCP _unicode) ;;supports redirected
else inp- s
s.trim("[]")
