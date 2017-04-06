 http://www.99-bottles-of-beer.net

out
 __________________________________

;Ninety-nine Bottles of Beer in Quick Macros

str longString=
 no more
 one
 two
 three
 four
 five
 six
 seven
 eight
 nine
 ten
 eleven
 twelve
 thirteen
 fourteen
 fifteen
 sixteen
 seventeen
 eighteen
 nineteen
ARRAY(str) a1=longString
ARRAY(str) a2="[][]twenty[]thirty[]forty[]fifty[]sixty[]seventy[]eighty[]ninety"

str s s1 s2
int i i1 i2

for i 99 -1 -1
	i1=i/10; i2=i%10
	
	if(i<20) s1=a1[i]
	else if(i2) s1.from(a2[i1] "-" a1[i2])
	else s1=a2[i1]
	
	if(i!99) s.addline(F"Take one down and pass it around, {s1} bottles of beer on the wall.[]")
	s2.format("%s bottles of beer on the wall, %s bottles of beer." s1 s1)
	s2[0]=toupper(s2[0])
	s.addline(s2)

s.replacerx("(one bottle)s" "$1" 1)
s.addline("Go to the store and buy some more, ninety-nine bottles of beer on the wall.")

 Show the song

sel list("QM output[]Notepad" "Show the song in")
	case 1
	out s
	
	case 2
	run "notepad.exe" "" "" "" 0x2800 win("Notepad" "Notepad")
	s.setsel

 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678
