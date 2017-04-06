dll "qm.exe"
	#_Test0
	#_Test1 a
	#_Test2 a b
	#_Test3 a b c
	#_Test4 a b c d
int i j
str s.all(10000 2)
ARRAY(int) a.create(10000)

WakeCPU
PF
 loop
for(i 0 10000) j+1
PN
 if, expression
for(i 0 10000) if(j) j=j && j*2+1 > 1;
PN
 string iteration
for(i 0 10000) j+=s[i]
PN
 array iteration
for(i 0 10000) j+=a[i]
PN
 user function call
for(i 0 10000)
	j=sub.Test
	j=sub.Test1(i)
	j=sub.Test2(i 0)
	j=sub.Test3(i j 0)
	j=sub.Test4(i j 1000000 0)
PN
 dll function call
for(i 0 10000)
	j=_Test0
	j=_Test1(i)
	j=_Test2(i 0)
	j=_Test3(i j 0)
	j=_Test4(i j 1000000 0)
PN
PO


#sub Test
ret 1

#sub Test1
function a
ret 1

#sub Test2
function a b
ret 1

#sub Test3
function a b c
ret 1

#sub Test4
function a b c d
ret 1
