DeleteFileCacheAll "Q:"
1
PF
 wait 0 H mac("Function253")
 _s.getfile("$qm$\ok.qml")

 _s.getfile("$qm$\sqlite3.dll")

int hm=LoadLibraryW(L"q:\app\sqlite3.dll")
PN
GetProcAddress(hm "sqlite3_exec")
PN; PO

 out hm
FreeLibrary hm
