/exe 2
out _portable
out GetEnvVar("app" _s)
out _s.expandpath("$my qm$")

STARTUPINFO si
GetStartupInfo &si
 out _s.getstruct(si 1)
out si.wShowWindow

 lpstr s=GetEnvironmentStrings
 rep
	 if(!s[0]) break
	 out s
	 s+len(s)+1
 FreeEnvironmentStrings(s)

 BEGIN PROJECT
 main_function  Macro2110
 exe_file  $temp$\Macro2111.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {AB1F5E09-4CD5-4FEC-A378-944EF0D92B0C}
 END PROJECT

 SEE_MASK_UNICODE|SEE_MASK_NO_CONSOLE
