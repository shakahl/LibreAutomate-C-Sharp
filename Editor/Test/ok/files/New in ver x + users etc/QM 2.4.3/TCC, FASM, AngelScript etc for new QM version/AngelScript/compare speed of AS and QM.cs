dll "qm.exe" #TestAdd a b

WakeCPU
PF
int i j
 rep 1000000
for j 0 100000
	i=TestAdd(j 5)
	 i=sub.TestAdd(j 5)
PN
 str s.all(100000 2 'a')
 for j 0 s.len
	 if(s[j]='k') break
ARRAY(byte) a.create(100000)
for j 0 a.len
	if(a[j]='k') i+1
PN
PO


#sub TestAdd
function# a b
ret a+b

 simple loop + simple expression:
 MSVC optimized 0.4
 MSVC no optimizations 2.4
 TCC 2.4
 QM 34
 AS 13
 AS JIT 3.3
 MS JSsript 130

 system function call:
 MSVC optimized 
 MSVC no optimizations 
 TCC 
 QM 3.7 ms
 AS 7ms, asCALL_GENERIC 6.3ms
 AS JIT 3.3

 script function call
 TCC 
 QM 10
 AS 4
 AS JIT 3.4

 string loop
 QM 4.5
 AS 3
 AS JIT 

 array loop
 QM 5.5
 AS 3
 AS JIT 
