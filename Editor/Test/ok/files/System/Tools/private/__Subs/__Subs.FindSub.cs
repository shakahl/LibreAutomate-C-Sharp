function# $name

 Returns index of sub-function.
 Returns -1 if not found.

if(empty(name)) ret 0
int i
for(i 1 a.len) if(a[i].name=name) ret i
ret -1
