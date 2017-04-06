 /
function# $exename str&path

 Gets full path of a program that registers itself in 'App Paths' registry key when installing.
 Returns 1 if found, 0 if not.

 exename - program filename. Can contain wildcard characters. For example, "forge??.exe" would find "forge80.exe", "forge90.exe" etc.
 path - str variable that receives full path of the program file.

 EXAMPLE
 str path
 if(GetInstalledProgramPath("qm.exe" path))
	 out path
 else
	 out "not installed"


str ap="SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths"
ARRAY(str) a; int i
RegGetSubkeys a ap HKEY_LOCAL_MACHINE
for i 0 a.len
	if(matchw(a[i] exename 1))
		ret(rget(path "" _s.from(ap "\" a[i]) HKEY_LOCAL_MACHINE)>1)
path.all

 TODO: WaitForWindowWithAcc (now we have WinA).
