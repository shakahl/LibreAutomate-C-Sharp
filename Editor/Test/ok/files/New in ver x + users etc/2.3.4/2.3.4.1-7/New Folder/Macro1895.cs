 /exe

 mou 1 1

int i
 IStringMap m
  m=CreateStringMap
 m._create
 m.AddList("a,1[]b,2" "csv")
 out m.Get("b")

 ICsv x._create
 x.FromString("a,   1[]b   ,''c''")
 x.ToString(_s)
 out _s

 IXml x._create
 x.FromString("<a>dd</a>")
 x.ToString(_s)
 out _s


 BEGIN PROJECT
 main_function  Macro1895
 exe_file  $my qm$\Macro1895.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {F982178A-4DD3-406D-9020-5A329CEDC131}
 END PROJECT
