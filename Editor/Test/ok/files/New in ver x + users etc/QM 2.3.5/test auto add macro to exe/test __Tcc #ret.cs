/exe
__Tcc t.Compile("" "Test")
 __Tcc t.Compile("macro:test __Tcc #ret" "Test")
 __Tcc t.Compile("*test __Tcc #ret" "Test")
 __Tcc t.Compile("*testhhh" "Test")
 __Tcc t.Compile("macro:testhhh" "Test")
 __Tcc t.Compile("void Test(){q_printf(''sss'');}" "Test")
 _s="void Test(){q_printf(''sss'');}"; __Tcc t.Compile(_s "Test")
call(t.f)

 BEGIN PROJECT
 main_function  test __Tcc #ret
 exe_file  $my qm$\test __Tcc #ret.qmm
 flags  6
 guid  {5C07F006-7B38-456B-9C6D-909991613B19}
 END PROJECT

#ret
void Test()
{
//bl=5;
q_printf("sss");
}
