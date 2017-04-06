 /exe
out
SetCurDir "c:\windows"
str qmPath;;="q:\app"

dll- kernel32 [SetDllDirectoryA]#SetDllDirectory $lpPathName
 err
Q &q
if rget(qmPath "" "Software\Microsoft\Windows\CurrentVersion\App Paths\qm.exe" HKEY_LOCAL_MACHINE)
	out qmPath.getpath(qmPath "")
	 SetDllDirectory qmPath
	
	 if(GetEnvVar("PATH" _s))
		 out F"{_s};{qmPath}"
		 SetEnvVar("PATH" F"{_s} out _s
	SetEnvVar("PATH" qmPath)
Q &qq; outq

Q &q
int h=LoadLibraryW(L"qmtc32.dll")
Q &qq; outq
FreeLibrary h
out h
