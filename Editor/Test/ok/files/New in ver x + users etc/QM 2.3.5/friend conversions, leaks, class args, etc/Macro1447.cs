 /exe
out
ARRAY(str) a="one[]two"
out a.psa
VARIANT v=a
out "%i" v.parray
outx v.vt
 ARRAY(str)& r=v.parray
v.vt=0
ARRAY(str) b.psa=v.parray.psa
out b.psa
out b
 v.parray=0

 BEGIN PROJECT
 main_function  Macro1447
 exe_file  $my qm$\Macro1447.qmm
 flags  6
 guid  {4E634057-C0A8-4857-AB1A-CC20AC7527D2}
 END PROJECT
