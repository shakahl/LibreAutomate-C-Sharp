 ---- recorded (run, act, mouse, key)
 int w1=act(win("QM Help" "HH Parent"))
 lef 310 172 w1
 lef+ 225 355 w1
 lef- 395 354 w1
 'Cc
 run "notepad"
 int w2=wait(6 win("Untitled - Notepad" "Notepad"))
 'Cv
 lef 445 14 w2
 int w3=wait(5 win("Notepad" "#32770"))
 lef 121 105 w3
 act w1
 lef 79 54 w1

 ---- wait mouse, paste, copy, run
 wait 0 ML
 str s.getsel
 mes s
 run "notepad"
 s.setsel

 dialogs
 inpp "p"
 str s
 inp- s "" "" "[]"
 mes s
 list s

 ---- men, but
 men 2013 win("Quick Macros - ok - [Macro238]" "QM_Editor") ;;Find ...
 "win"
 but 1129 win

 ---- acc, htm
 act "MSDN"
 Acc a=acc("access token" "LINK" win("MSDN Library - July 2004 - SECURITY_ATTRIBUTES structure [Security]" "wndclass_desked_gsk") "Internet Explorer_Server" "" 0x1001)
 a.DoDefaultAction
 1
 Htm el=htm("I" "application protocol data unit" "" win("MSDN Library - July 2004 - A [Security]" "wndclass_desked_gsk") 0 16 0x21)
 el.Click

 ---- wait for
 run "notepad"
 0.5
 wait 0 WP "Notepad"

 wait 0 P

 run "http://www.yahoo.com"
 wait 0 I

 mes 1

 ---- regular expressions, wildcard
 str s="one two three"
 out matchw(s "*two*")
 out findrx(s "\btwo\b")
 s.replacerx("\btwo\b" "four")
 out s

 ---- program name
 str s.getwinexe(win("Visual") 1)
 out s
 act win("" "" "devenv")

 ---- 
