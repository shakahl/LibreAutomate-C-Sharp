 shows what properties the window has

out
int w=_hwndqm
 int w=win("Notepad")
 int w=win("jEdit - Untitled-1" "SunAwtFrame")
 int w=win("Global Options jEdit: General" "SunAwtDialog")
 int w=win("Untitled 1 - OpenOffice Writer" "SALFRAME")
 int w=win("TB INTERNET" "QM_toolbar")

ARRAY(STRINT) a; int i
GetAllProp w a
for(i 0 a.len) out a[i].s
