 /bug return str from array
function'str

ARRAY(str) a="one[]two"
 out "%i" a[0].lpstr
 ret a[0]

str d=a[0]
out "%i" d.lpstr
ret d
