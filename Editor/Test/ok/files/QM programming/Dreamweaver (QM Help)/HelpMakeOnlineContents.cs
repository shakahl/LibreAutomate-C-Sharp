str s.getfile("$qm$\htmlhelp\qm2help.hhc") ss.getfile("$qm$\htmlhelp\left.htm") head

findrx(ss "(?s)^.+?<UL>" 0 0 head)

s.replacerx("(?s)^.+?<UL>" head 4)

s.replacerx("<OBJECT.+?\n.+?value=''(.+?)''>\s+</OBJECT>" "<i>$1</i>")
s.replacerx("(?s)<OBJECT.+?value=''(.+?)''.+?value=(.+?)>\s+</OBJECT>" "<a href=$2 target=right>$1</a> </LI>")
s.findreplace("\" "/")

s.setfile("$qm$\htmlhelp\left.htm")
 ShowText "" s
