/exe
out
int i j

 ARRAY(int) a.create(10)
 ARRAY(byte) a.create(10)
 ARRAY(word) a.create(10)
 ARRAY(long) a.create(10)
 ARRAY(double) a.create(10)
 for i 0 a.len
	  a[i]=RandomInt
	 a[i]=RandomDouble(-1000000 1000000)

ARRAY(str) a.create(10)
for i 0 a.len
	a[i].RandomString(3 15 "a-zA-Z")

 str rs.RandomString(22 22 "a-zA-Z")
 out rs; out "----------"
 ARRAY(lpstr) a.create(10)
 for i 0 a.len
	 a[i]=rs+RandomInt(0 20)
 a[3]=0
 a[4]=""

 ARRAY(BSTR) a.create(10)
 for i 0 a.len
	 a[i]=_s.RandomString(3 15 "a-zA-Z")

 ARRAY(POSTFIELD) a.create(10)
 for i 0 a.len
	 a[i].value=_s.RandomString(3 15 "a-zA-Z")

 Q &q
 a.sort
 a.sort(1)
 a.sort(2)
 a.sort(3)
 a.sort(0 sort_proc_int 100) ;;10.5 times slower
 a.sort(0 sort_proc_byte 100)
 a.sort(0 sort_proc_long 100)
 a.sort(0 sort_proc_double 100)
a.sort(0 sort_proc_str 100) ;;7.5 times slower
 POINT p.y=2
 a.sort(0 sort_proc_BSTR &p)
 a.sort(0 sort_proc_POSTFIELD 100)
 a.sort(0 0 100)
 out a.sort(2)
 Q &qq
 outq

for i 0 a.len
	out a[i]
 for i 0 a.len
	 out a[i].value
	
 BEGIN PROJECT
 main_function  Macro653
 exe_file  $my qm$\Macro653.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {C347D63E-5030-486A-8EEB-548168DAE1A7}
 END PROJECT
