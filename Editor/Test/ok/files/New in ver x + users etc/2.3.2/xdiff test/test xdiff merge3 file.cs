out
str f0="$temp$\s0.txt"
str f1="$temp$\s1.txt"
str f2="$temp$\s2.txt"
str s0="one[]two[]three[]four[]five[]"; s0.setfile(f0)
str s1="one[]added1[]two[]three[]four[]five[]"; s1.setfile(f1)
str s2="one[]two[]three[]four[]added2[]five[]"; s2.setfile(f2)
str sRej
str fMerged="$temp$\merged.txt"

#compile "__Xdiff"
Xdiff x

 Q &q
x.Merge3TextFile(f0 f1 f2 fMerged sRej)
 Q &qq; outq

out "---- new ----[]%s[]---- rej ----[]%s" _s.getfile(fMerged) sRej
