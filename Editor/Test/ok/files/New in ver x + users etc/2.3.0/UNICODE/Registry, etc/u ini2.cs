out
str ini="$desktop$\ąč ﯔﮥ k.ini"
str s="ąč ﯔﮥ k"

rset s "ąč ﯔﮥ k" "ąč ﯔﮥ k" ini

str ss
out rget(ss "ąč ﯔﮥ k" "ąč ﯔﮥ k" ini)
out ss

out rget(ss "ąč ﯔﮥ k no" "ąč ﯔﮥ k" ini "no ąč ﯔﮥ k")
out ss

 out rset("" "ąč ﯔﮥ k" "ąč ﯔﮥ k" ini -1)
out rset("" "" "ąč ﯔﮥ k" ini -2)
