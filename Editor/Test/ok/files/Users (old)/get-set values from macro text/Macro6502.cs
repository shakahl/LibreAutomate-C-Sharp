str s s1 s2
 write two values
s="value1"; s.ValueFromMacro("passwords" "name1" 1)
s="value2"; s.ValueFromMacro("passwords" "name2" 1)
 read two values
s1.ValueFromMacro("passwords" "name1")
s1.ValueFromMacro("passwords" "name1")
s2.ValueFromMacro("passwords" "name2")
out "%s %s" s1 s2
