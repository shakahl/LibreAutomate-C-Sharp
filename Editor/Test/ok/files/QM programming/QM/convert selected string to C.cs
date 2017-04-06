str s.getsel
s.trim("''")
if(!s.len) ret
s.findreplace("\" "\\")
s.findreplace("[39]'" "\''")
s.findreplace("[91]]" "\r\n")

 note: does not replace escape sequences like [10]
out s
s.setclip
out "(also copied to clipboard)"
