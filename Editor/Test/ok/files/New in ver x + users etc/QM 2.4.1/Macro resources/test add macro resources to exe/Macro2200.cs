 /exe
 #exe addfile "$myqm$\test\test.txt" 2
 out ExeGetResourceData(10 2 &_s)
 out _s

 #exe addfile "$myqm$\test\test.txt" 2 50
 out ExeGetResourceData(50 2 &_s)
 out _s

 #exe addfile "$myqm$\test\test.txt" "name" "type"
 out ExeGetResourceData("type" "name" &_s)
  out ExeGetResourceData(L"type" L"name" &_s)
 out _s

  #exe addfile "$myqm$\test\test.txt" 2 500000
 #exe addfile "$myqm$\test\test.txt" 500000 2
 #exe addfile "$myqm$\test\te st.txt" 2 50
 out ExeGetResourceData(50 2 &_s)
 out _s


 out _s.getfile(":2 $myqm$\test\te st.txt")


 str s="image:test"
 str s="resource:<resource is here>image:test"
 str s="resource:<resource is here>image:hEA3AC2B7"

 BEGIN PROJECT
 main_function  Macro2200
 exe_file  $my qm$\Macro2200.qmm
 flags  23
 guid  {F3DE7FBE-2563-4014-8B96-7235FEABE940}
 END PROJECT
