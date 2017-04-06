out
str s0="one[]two[]three[]four[]five[]"
 str s1="one[]added1[]two[]three[]four[]five[]"
 str s2="one[]two[]three[]four[]added2[]five[]"
 str s1="one[]changed1[]three[]four[]five[]"
 str s2="one[]two[]three[]changed2[]five[]"
 str s1="one[]two[]three[]four[]five[]added1[]added11[]"
 str s2="one[]two[]four[]five[]added2[]added22[]"
str s1="one[]two[]three[]four[]five[]"
str s2="one[]two[]three[]changed2[]five[]"
str sMerged sRej

#compile "__Xdiff"
Xdiff x

#if 1
 Q &q
x.Merge3Text(s0 s1 s2 sMerged sRej)
 Q &qq; outq
#else
str sd
x.DiffText(s0 s2 sd)
 out sd
x.PatchText(s1 sd sMerged 0 sRej)
#endif
out "---- new ----[]%s[]---- rej ----[]%s" sMerged sRej
