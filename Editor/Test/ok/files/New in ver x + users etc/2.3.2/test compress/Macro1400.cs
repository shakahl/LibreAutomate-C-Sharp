out
str s.getmacro("BitmapFileToMacro")
out s.len

 str ss.encrypt(32|4 s)
str ss.encrypt(32|8 s)
out ss.len
outb ss ss.len 1

 s.decrypt(32|4 ss)
s.decrypt(32|8 ss)
out s.len
out s
