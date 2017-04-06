out
str fPatch="$temp$\xdiff_patch.txt"

str f1="$qm$\qm2.exe"
str f2="$qm$\qm.exe"

 str f1="$temp$\s1.txt"
 str f2="$temp$\s2.txt"
 str s1="one1[]one2[]two[]three[]four1[]four2[]five[]six[]eight[]"; s1.setfile(f1)
 str s2="one1[]one2[]twoo[]thre[]four1[]four2[]six[]seven[]eight[]"; s2.setfile(f2)

_s.getfile(f2); out "%i %i" _s.len Crc32(_s _s.len)

#compile "__Xdiff"
Xdiff x

Q &q
x.DiffBinaryFile(f1 f2 fPatch)
Q &qq; outq
 _s.getfile(fPatch); outb _s _s.len 1

out "---------------------"

str fNew2="$temp$\xdiff_new2.txt"
x.PatchBinaryFile(f1 fPatch fNew2)
_s.getfile(fNew2); out "%i %i" _s.len Crc32(_s _s.len)
