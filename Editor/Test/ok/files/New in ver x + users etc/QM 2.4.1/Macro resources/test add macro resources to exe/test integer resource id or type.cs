 /exe

#if !EXE
_qmfile.ResourceAdd(+-3 "7" "DATA" 4)
_qmfile.ResourceAdd(+-3 "44:8" "DATA2" 5)
_qmfile.ResourceAdd(+-3 "0xF" "DATA3" 5)
_qmfile.ResourceAdd(+-3 "str:ing" "DATA4" 5)
#else
str s1="resource:7"
str s2="resource:44:8"
str s3="resource:0xF"
str s4="resource:str:ing"
 out _s.getfile(":7")
if(ExeGetResourceData(10 7 &_s)) out _s
if(ExeGetResourceData(44 8 &_s)) out _s
if(ExeGetResourceData(10 0xF &_s)) out _s
if(ExeGetResourceData("str" "ing" &_s)) out _s

 BEGIN PROJECT
 main_function  test integer resource id or type
 exe_file  $my qm$\Macro2214.qmm
 flags  7
 guid  {51B38B12-70DE-4B09-97AF-3A711851DBE2}
 END PROJECT
