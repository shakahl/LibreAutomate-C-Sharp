out
str s="data"
__TempFile x
 out x.Init
 out x.Init(".txt")
 out x.Init(".txt" "f")
out x.Init(".txt" "" "\subfolder")
 out x.Init(".txt" "f" "$temp$")
 out x.Init(".txt" "" "$temp$")
 out x.Init(".txt" "" "" s)
 mes "waiting"
