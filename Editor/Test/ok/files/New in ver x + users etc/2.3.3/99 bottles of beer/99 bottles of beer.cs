 http://www.99-bottles-of-beer.net

out
 __________________________________

 99 Bottles of Beer in Quick Macros

str s
int i
for i 99 -1 -1
	if(i!99) s.addline(F"Take one down and pass it around, {i} bottles of beer on the wall.[]")
	s.addline(F"{i} bottles of beer on the wall, {i} bottles of beer.")
s.addline("Go to the store and buy some more, 99 bottles of beer on the wall.")
s.replacerx("(1 bottle)s" "$1" 2)
s.findreplace("[]0" "[]No more"); s.findreplace("0" "no more" 2)

out s ;;show the song in Quick Macros

 Quick Macros is an automation program, and therefore text often goes to some window...
run "notepad.exe" "" "" "" 0x2800 win("Notepad" "Notepad") ;;run notepad
s.setsel ;;paste the song


 __________________________________

 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678
 The above line has 100 characters. It is the code line length limit for the site.
