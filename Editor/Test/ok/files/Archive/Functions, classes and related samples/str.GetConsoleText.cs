 /
function# [hwnd] [firstline] [nlines]

 Gets text of a console window.
 Returns total number of lines. On failure returns 0.
 Requires Windows XP or later.

 An example of console window is Windows command prompt.
 Alternatively can be used RunConsole or RunConsole2, which runs the program and gets its output.
 Other functions (getwintext, acc, etc) cannot get console window text.

 hwnd - console window handle. If omitted or 0, uses first found console window.
 firstline - 0-based index of first line to get. Default: 0. If firstline >= total number of lines, the function will succeed but the str variable will be empty.
 nlines - number of lines to get. Default: to the end. If firstline+nlines > total number of lines, the function will succeed and get all lines until the end.

 EXAMPLE
 str s
 if(!s.GetConsoleText(0)) ret
 out s


fix(0)
if(!hwnd) hwnd=win("" "ConsoleWindowClass"); if(!hwnd) ret
if(!GetWindowThreadProcessId(hwnd &_i)) ret
lock
FreeConsole
if(!AttachConsole(_i)) ret

int ho=GetStdHandle(STD_OUTPUT_HANDLE)

CONSOLE_SCREEN_BUFFER_INFO cb
if(!GetConsoleScreenBufferInfo(ho &cb)) goto ge

int i n nc(cb.dwSize.X) nl(cb.dwCursorPosition.Y+1) stopline
if(firstline<nl)
	if(!nlines) nlines=1000000000
	if(nlines>nl-firstline) nlines=nl-firstline
	stopline=firstline+nlines
	
	str s.all(nc); s.flags=1
	for i firstline stopline
		COORD c.Y=i
		if(!ReadConsoleOutputCharacter(ho s nc c &n)); goto ge
		s.fix(n); s.rtrim
		addline(s i=stopline-1)

FreeConsole
ret nl
 ge
FreeConsole
