out
str fPatch="$temp$\xdiff_patch.txt"

 str f1="$desktop$\threads.cpp"
 str f2="$desktop$\threads (2).cpp"

str f1="$temp$\s1.txt"
str f2="$temp$\s2.txt"
str s1="one1[]one2[]two[]three[]four1[]four2[]five[]six[]eight[]"; s1.setfile(f1)
str s2="one1[]one2[]twoo[]thre[]four1[]four2[]six[]seven[]eight[]"; s2.setfile(f2)

#compile "__Xdiff"
Xdiff x

 x.FileHeader(f1 f2 _s); out _s

x.DiffTextFile(f1 f2 fPatch 1)
out _s.getfile(fPatch)

out "---------------------"

str fNew2="$temp$\xdiff_new2.txt"
str sRej
x.PatchTextFile(f1 fPatch fNew2 0 sRej)
 x.PatchTextFile(f2 fPatch fNew2 1 sRej) ;;reverse
out "---- new ----[]%s[]---- rej ----[]%s" _s.getfile(fNew2) sRej
