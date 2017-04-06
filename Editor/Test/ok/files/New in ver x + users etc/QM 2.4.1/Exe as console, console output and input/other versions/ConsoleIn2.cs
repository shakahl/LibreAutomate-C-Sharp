 /
function str&s

 In exe waits and reads user input text from console window.

 s - variable that receives the text (1 line).

 REMARKS
 Exe must be created as console. For it you need function MakeExe_Console. Put its name in 'Make exe' dialog, 'After' field.
 If exe runs from cmd.exe, can use its console. Else exe creates own console window when starting.
 If called in QM (not in exe), this function just calls inp().
 This function waits for Enter. While waiting, cannot process Windows messages and COM events. The thread should not have dialogs etc.

 Example: <open>QM-console</open>.


s.all
#if EXE!1
if(!inp(s)) end "Cancel"
#else
int+ g_stdin
if !g_stdin
	g_stdin=GetStdHandle(STD_INPUT_HANDLE)

_s.all(16*1024 2)

if(!ReadConsole(g_stdin _s _s.len &_i 0)) end "failed" 16
_s.fix(_i); _s.trim("[]")
if(_unicode) _s.ConvertEncoding(GetConsoleCP _unicode)
s=_s
#endif

 tested: _cgets fails.
