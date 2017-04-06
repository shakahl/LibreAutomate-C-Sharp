out
str s1="one1[]one2[]two[]three[]four1[]four2[]five[]six[]eight[]"
str s2="one1[]one2[]twoo[]thre[]four1[]four2[]six[]seven[]eight[]"
 str s1.getfile("$desktop$\threads.cpp")
 str s2.getfile("$desktop$\threads (2).cpp")
str s3
 rep(5) s1+s1; s2+s2

#compile "__Xdiff"
Xdiff x

 Q &q
x.DiffText(s1 s2 s3 0)
 Q &qq; outq
out s3
 out "%i %i %i" s1.len s2.len s3.len
ret

out "---------------------"
#if 0 ;;test reject
x.DiffText(_s.from(s1 "nine[]ten[]") s2 s3 0)
#endif

str sNew2 sRej
x.PatchText(s1 s3 sNew2 0 sRej)
 x.PatchText(s2 s3 sNew2 1 sRej) ;;reverse
out "---- new ----[]%s[]---- rej ----[]%s" sNew2 sRej
