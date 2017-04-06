str s=
 zero,aaa
 one,bbb
 two,ccc
ICsv c._create
c.FromString(s)

 out c.Find("one")
 out c.Find("One" 1)
 out c.Find("o" 4)
 out c.Find("*n?" 2)
 out c.Find("e" 8)
 out c.Find("n" 16)
 out c.Find(".n." 32)
out c.Find("ccc" 0 1)
