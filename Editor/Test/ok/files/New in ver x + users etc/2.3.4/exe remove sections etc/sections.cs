 /exe
out 1

_s="worked"
 out _s.stem

_s="one two"
 out _s.wrap(6)

 acc

 VARIANT v
 v=_s
 v=5
 int x=v
 double x=v
 long x=v
 int* x=+v
 out x
 v=-v
 v=~v
 v=!v
 v.add(10)
 out v

 DATE d.getclock
 SYSTEMTIME st.wDay=1
 d.add(st)
 out d

 int* p=v
 out p

 wait 0 I

 out _s.encrypt(1|8 _s "kmm")
 out _s.decrypt(1|8 _s "kmm")

 out net("localhost" "[*FE27B7172700A05201*]" "sections_net")

 inpp "[*C44338979A4CBD5205*]" "" "" 1

 Htm el=htm("a" "ddddd")
 acc

 key k
 key+ C
 paste "k"
 outp "k"
 _s.setsel
 _s.getsel
 _s.getclip
 _s.setclip

 out pixel(0 0)
 mou 10 100
 lef 300 200 win
 dou 300 200 win

 scan "sections.bmp" 0 0 1|2
 wait 0 S "sections.bmp" 0 0 1|2

 IXml x=CreateXml
 x.Add("x" "nnnn")
 x.ToString(_s); out _s

out 2

 BEGIN PROJECT
 main_function  sections
 exe_file  $my qm$\sections.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {120A5472-46A6-434C-BB99-31F3F1CD594E}
 END PROJECT

#ret
_s.