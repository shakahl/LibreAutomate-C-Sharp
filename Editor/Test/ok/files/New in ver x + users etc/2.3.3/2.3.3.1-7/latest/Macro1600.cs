 out findw("aaa ,bbb, ccc" " ,Bbb" 0 "" 1|0)
 out findw("aaa ,bbb, ccc" "Bbb, " 0 "" 1|2)
 out findw("aaa ,bbb ccc" "Bbb" 0 "," 1|0)
 out findw("aaa ,bbb ccc" "Bbb" 0 "," 1|4)


str s.all(256 2 0); s[0]=1; s[',']=1
 s[32]=1
 out findw("aaa ,bbb ccc" "Bbb" 0 s 1|4|0x100)
 out findw("aaa ,bbb ccc" "Bbb" 0 s 1|0x100)

Q &q
rep(1000) findw("aaa ,bbb ccc" "Bbb" 0 "," 5)
Q &qq
rep(1000) findw("aaa ,bbb ccc" "Bbb" 0 s 0x105)
Q &qqq
outq

