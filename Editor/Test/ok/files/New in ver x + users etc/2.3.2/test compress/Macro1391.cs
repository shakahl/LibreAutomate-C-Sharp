out
str s.getmacro("BitmapFileToMacro")
out s.len

str ss.encrypt(32 s)
out ss.len
outb ss ss.len 1

s.decrypt(32 ss)
out s.len
out s
