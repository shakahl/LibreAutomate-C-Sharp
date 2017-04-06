 dll "$qm$\qmplus"
	 #TestQmplus x
	
 out TestQmplus(5)

out
str s=
 aaaaaaaaaaaaaaa, bbbbbbbbbbbbbbbbbbbbbbb ccccccccccccccccc
 ddddddddd eeeeeeeeeeeeee, fffffffffffffffffffff, gggggggg

 out s.wrap(30)
 out s.wrap(30 "-")
 out s.wrap(30 "" ",")
 out s.wrap(30 "" "" 1|2)
 out s.wrap(30 "" "" 0 30)
 ___________________

 s="working"
 out s.stem

 PF
 rep(1000) qmplus_str_stem &s 0 0 0
 PN
 rep(1000) s.stem
 PN;PO
 ___________________

str s1 s2
s1="ddddddddddd"
 s1.Swap(s2)
s1.swap(s2)
out s1
out s2
