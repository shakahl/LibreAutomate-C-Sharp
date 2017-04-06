 Creates console window and redirects output of msvcrt.dll functions (printf etc) to it.

 create console window
if(!AllocConsole()) ret ;;probably already set

 get stdout
dll MSVCRT _iob
FILE* stdhandles=&_iob
FILE* stdout=&stdhandles[1]

 redirect
int hCrt = _open_osfhandle(GetStdHandle(STD_OUTPUT_HANDLE), _O_TEXT)
FILE* hf = _fdopen(hCrt, "w")
*stdout = *hf
setvbuf(+stdout, 0, _IONBF, 0)
