out
ICsv c=CreateCsv

lpstr s1("1") s2("2") s3("3") s4("4")
c.AddRowLA(0 4 &s1)

str ss1("11") ss2("22") ss3("33") ss4("44")
c.AddRowSA(0 4 &ss1)
c.AddRowSA(2 4 &ss1)

str s="abcdefghijklmn"
s[3]=0
s[7]=0
s[11]=0
c.AddRowMS(-1 4 s)
c.AddRowMS(100 4 s)


str ss
c.ToString(ss)
out ss
