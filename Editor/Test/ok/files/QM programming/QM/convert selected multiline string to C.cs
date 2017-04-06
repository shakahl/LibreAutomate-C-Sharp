str s.getsel
if(!s.len) ret
s.replacerx("(?m)^ +")
s.findreplace("\" "\\")
s.findreplace("''" "\''")
s.findreplace("[]" "\r\n''[]''")
s.findreplace("[9]" "\t")
s.rtrim("''[]")
s-"''"; s+"'';"
out s
s.setclip
out "(also copied to clipboard)"
