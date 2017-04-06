out
str s.getsel
 s.replacerx("^[^[]]+$" "<p>$0</p>" 8)
s.findreplace("[]" "[][]")
s.setclip

