out

str s="Price1 - $1.1    Price2 - €2.2"

 str pattern=".+ \- [$€£](\d.+) .+ \- [$€£](\d.+)"
str pattern=".+ \- (?:\$|€|£)(\d.+) .+ \- (?:\$|€|£)(\d.+)"

ARRAY(str) arr
if(findrx(s pattern 0 0 arr)<0) out "not found"; end

out arr[1]
out arr[2]
