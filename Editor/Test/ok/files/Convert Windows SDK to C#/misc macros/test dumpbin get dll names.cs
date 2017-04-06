 runs dumpbin.exe for a single lib file, to show what dll functions (and their dll) are there

out
str lib="Q:\SDK10\Lib\10.0.10586.0\um\x64\pla.lib"
 out FileExists(lib)
str dumpbin_exe="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\dumpbin.exe"
str s
int e=RunConsole2(F"''{dumpbin_exe}'' /HEADERS ''{lib}''" s)
if(e) out e; ret
 out s
str sf="$temp qm$\dumpbin2.txt"
s.setfile(sf)
run "$qm$\qm.exe" sf
