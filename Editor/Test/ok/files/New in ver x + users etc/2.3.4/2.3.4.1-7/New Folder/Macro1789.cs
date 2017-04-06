out
str s="HELLO WORLD ~ hello world"

 s.replacerx("(.+) ~ (.+)" "$2 ~ $1")

REPLACERX r.frepl=&Callback_str_replacerx2
 s.replacerx("(.+) ~ (.+)" r)
s.replacerx("(\w+) ~ (\w+)" r)

out s
