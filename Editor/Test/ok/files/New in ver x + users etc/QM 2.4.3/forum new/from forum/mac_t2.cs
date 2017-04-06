/
function x ARRAY(int)&y

atend sub.AutoDeleteArray &y

out x
out y[1]
wait 2


#sub AutoDeleteArray
function ARRAY(int)*p
p._delete
out "array deleted"
