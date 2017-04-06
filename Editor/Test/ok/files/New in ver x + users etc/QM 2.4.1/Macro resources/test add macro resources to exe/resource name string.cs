 /exe
 #exe addfile "$my qm$\test\test.txt" 50000 50000
#exe addfile "$my qm$\test\test.txt" 50000 "Res type+"
#exe addfile "$my qm$\test\test.txt" "Res name+" 50000
 #exe addfile "$my qm$\test\test.txt" "rName" "rTyp"
#exe addfile "$my qm$\test\test2.txt" "rNamE" "rTyP"
 out
 out 1
str s1 s2 s3 s4 s5
 ExeGetResourceData 50000 50000 &_s; out _s
if(ExeGetResourceData(L"Res TYPE+" 50000 &s1)) out s1; else out "no 1"
if(ExeGetResourceData(50000 L"Res NAME+" &s2)) out s2; else out "no 2"
if(ExeGetResourceData(L"TEST" 100 &s3)) out s3; else out "no 3"
if(ExeGetResourceData(L"TEST" L"NameX" &s4)) out s4; else out "no 4"
if(ExeGetResourceData(L"rTyp" L"rName" &s5)) out s5; else out "no 5"
 str s=":50000 $my qm$\test\test.txt"
 ExeGetResourceData 10 50000 &_s; out _s


 BEGIN PROJECT
 main_function  Macro2194
 exe_file  $my qm$\Macro2194.qmm
 res  $my qm$\test\test.res
 flags  6
 guid  {8FFC3E37-2780-4B3E-8FCE-464B64939AE3}
 END PROJECT
