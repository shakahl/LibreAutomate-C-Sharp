 mes("10")
 mes(10)
 str s="kk"
 mes("ab %% %s" "" "" s)
 mes("a" "t")
 mes("a" "" "YN")
 mes("" "" "YNC2?")
 if(mes("" "" "YNa")!='Y') ret
 mes-("" "" "OC")
 sel(mes("" "" "ARI!"))
	 case 'A' out 1
	 case 'R' out 2
	 case 'I' out 3
 mes("" "" "n")

 str s
 if(!inp(s)) ret
 if(!inp(_s "descr" "tit")) ret
 out _s
 int i
 inp(i "" "" "10")
 out i
 double i=5.5
 if(!inp(i "" "" i)) ret
 out i
 inp(t)

 sel(list("a[]b" "d[]y[]y" "t" 1 1 10 2 0x1))
	 case 1 out 1
	 case 2 out 2
	 case else ret

