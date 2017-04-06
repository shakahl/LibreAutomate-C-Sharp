out
 str s.expandpath("$qm$\qm.exe")
str s.expandpath("$my qm$\Macro1843.exe")

str b.all(10000 2)
if(!GetFileVersionInfo(s 0 b.len b)) ret
out 1

VS_FIXEDFILEINFO* vi
if(!VerQueryValue(b "\" &vi &_i)) ret
out 2

