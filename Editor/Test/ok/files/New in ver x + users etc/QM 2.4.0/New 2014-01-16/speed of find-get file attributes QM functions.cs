str s="q:\my qm\test\test.txt"
 str s=":4 q:\my qm\test\test.txt"
int r1 r2 r3 r4
rep(1000) r1=iff(s)
PF
rep(1000) r1=iff(s)
PN
rep(1000) r2=dir(s)
PN
rep(1000) r3=GetFileAttributesW(@s)
PN
 rep(1000) r4=_s.searchpath(s)
 rep(1000) WIN32_FIND_DATAW f; r4=FindFirstFileW(@s &f); FindClose(r4)
rep(1000) r4=FileExists(s)
PN
PO
out "%i %i %i %i" r1 r2 r3 r4
