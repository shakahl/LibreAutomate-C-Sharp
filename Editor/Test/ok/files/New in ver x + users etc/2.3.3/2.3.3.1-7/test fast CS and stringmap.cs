 dll "qm.exe" TestCsNewMT
 TestCsNewMT
 dll "qm.exe" TestLockTT
 TestLockTT
dll "qm.exe" TestSM IStringMap'm $k $*v

IStringMap m=CreateStringMap
str s=
 one ONE
 two TWO
 three THREE
 four FOUR
 five FIVE
 six SIX
 seven SEVEN
 eith EITH
 nine NINE
 ten TEN

m.AddList(s)

lpstr v
TestSM m "eith" &v
out v
