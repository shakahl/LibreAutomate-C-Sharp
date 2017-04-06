out
str s.getmacro("BitmapFileToMacro")
out s.len

 s.encrypt(32|4)
s.encrypt(32|8)
out s.len
outb s s.len 1

 s.decrypt(32|4)
s.decrypt(32|8)
out s.len
out s
