 wait 5
 0 "+BaseBar"
 5 WA "+BaseBar"
 0 WAI "Notepad[]Calc"
 0 id(100 win("Favorites" "ExploreWClass"))
 0 WN "Quick"
 0 -WA "Quick"
 0 WC "Calc"
 0 WD "Calc"
 0 -WC "Calc"
 0 WP "Calc"
 0 WV "Calc"
 0 -WV "Calc"
 0 WE win("Notepad" "Notepad")
 0 -WE win("Notepad" "Notepad")

 0 K k
 18 KF p
 0 ML
 0 MR
 0 MM
 0 M

 0 P 1
 0 -P 10

 wait 0 H mac("Function7")
 int h=run("notepad.exe")
 wait 0 H h; CloseHandle h
 
 int+ wi=0
 0 V wi

 wait 0 C 0xE55500 63 12 "Find"
 wait 0 -C 0xE7A580 171 21 "- Notepad"

 wait 0 I "http://www.quickmacros.com/index.html"

 str s="http://www.quickmacros.com/forum/index.php"
 web s
 0 I s

 wait 0 -CU IDC_ARROW
