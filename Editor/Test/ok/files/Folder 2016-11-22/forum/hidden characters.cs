out
str s

 normal
s="a"
out s.len
outb s s.len

 replaced from your string; the hidden characters remains
s="‎a"
out s.len
outb s s.len

 result
 1
 61 
 4
 E2 80 8E 61 

 s.findreplace("‎") ;;3 hidden bytes in ""
s.findreplace("[0xE2][0x80][0x8E]") ;;visual
out s
out s.len
outb s s.len
