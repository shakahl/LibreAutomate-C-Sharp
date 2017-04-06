type ___OUTC stdin stdout stderr conout __Handle'_conout __Handle'_conin !inited !redirected
___OUTC+ ___outc

if(___outc.inited) ret
lock
if(___outc.inited) ret
___outc.inited=1

___outc.stdout=GetStdHandle(STD_OUTPUT_HANDLE)
if !___outc.stdout and AllocConsole
	___outc.stdout=GetStdHandle(STD_OUTPUT_HANDLE)
	#if EXE=1
	end "no console. More info: <help>ExeConsoleWrite</help>." 8
	#endif
___outc.stderr=GetStdHandle(STD_ERROR_HANDLE)
___outc.stdin=GetStdHandle(STD_INPUT_HANDLE)

if(GetConsoleMode(___outc.stdout &_i)) ___outc.conout=___outc.stdout
else
	___outc.redirected=1
	___outc._conout=CreateFileW(L"CONOUT$" GENERIC_WRITE FILE_SHARE_READ|FILE_SHARE_WRITE 0 OPEN_EXISTING 0 0)
	___outc.conout=___outc._conout
	if(!___outc.stdin) ___outc._conin=CreateFileW(L"CONIN$" GENERIC_READ FILE_SHARE_READ|FILE_SHARE_WRITE 0 OPEN_EXISTING 0 0); ___outc.stdin=___outc._conin ;;eg RunConsole2
