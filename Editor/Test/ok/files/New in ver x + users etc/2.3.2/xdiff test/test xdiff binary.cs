out
 str s1="one1[]one2[]two[]three[]four1[]four2[]five[]six[]eight[]"
 str s2="one1[]one2[]twoo[]thre[]four1[]four2[]six[]seven[]eight[]"
str s1.getfile("$qm$\qm2.exe")
str s2.getfile("$qm$\qm.exe")
str s3
 rep(5) s1+s1; s2+s2
out "%i %i" s2.len Crc32(s2 s2.len)

#compile "__Xdiff"
Xdiff x

Q &q
x.DiffBinary(s1 s2 s3 1)
Q &qq; outq
 outb s3 s3.len 1
out "%i %i %i %i" s1.len s2.len s3.len Crc32(s3 s3.len)

out "---------------------"

str sNew2
x.PatchBinary(s1 s3 sNew2)
out sNew2
out "%i %i" sNew2.len Crc32(sNew2 sNew2.len)

 -1391931858
 1232896 1228800 792556 -2021135360
