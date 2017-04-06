str s="-1234567.1234567"
strrev s
s.replacerx("(\d{3})(?=\d+($|[^\.\w]))" "$1,")
strrev s
out s

