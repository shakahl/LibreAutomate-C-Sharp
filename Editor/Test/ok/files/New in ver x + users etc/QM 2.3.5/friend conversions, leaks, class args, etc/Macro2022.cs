dll "qm.exe" TestArr x ARRAY'a y
dll "qm.exe" [TestArr]TestArr2 ...
dll "qm.exe" [TestArr]TestArr3
 dll "qm.exe" TestArr ARRAY(BSTR)a x

 ARRAY(int) a.create(2); a[1]=7
ARRAY(str) a.create(2);; a[1]=7
 ARRAY(BSTR) a.create(2);; a[1]=7
 TestArr 1000 a 1000
 TestArr2 1000 a 1000
 TestArr3 1000 a 1000
 TestArr 1000 "one[]two" 2000
 VARIANT v=a
 VARIANT v.attach(a)
 TestArr 1000 v 1000
