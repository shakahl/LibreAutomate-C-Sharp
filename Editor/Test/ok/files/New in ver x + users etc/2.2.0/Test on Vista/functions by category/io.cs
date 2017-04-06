 Text dialog - everything OK (except in exe).

 _s="asdf"; _s.setfile("$Desktop$\a.txt" -1 -1 1)

 ShowText("" "adsasdads")

 act "Dialog"
 io.AutoPassword "user" "password" 1

 io.CapsLock 0
 key " a[]"

 act
 io.Enclose "<" ">" 1

 others tested previously
