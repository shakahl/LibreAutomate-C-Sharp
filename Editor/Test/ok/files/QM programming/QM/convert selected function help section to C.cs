str s.getsel
if(!s.len) ret
s.findreplace("[][]" "[]")
s.replacerx("(?m)^ " "//")

 note: does not replace escape sequences like [10]
out s
s.setclip
out "(also copied to clipboard)"
