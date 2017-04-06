__Hicon hi=GetFileIcon("$system$\compmgmt.msc")
out hi

 Call stack of thread Macro1557:
 mmcshext!0x723A425D
 util_file.cpp(320) : ufile::GetIconSpec(file=[0x1ED54F0], iconfile=[0x0], iconindex=[0x0], hi=[0x4B1FD50], cxy=[0x20], pidl=[0x93130]);  Locals: hr=[0xCCCCCCCC], hismall=[0x4B1FD50], hilarge=[0x0], pidlitem=[0x931D9], fl=[0x1A], ii=[0x0], eic=[0x36A890], folder=[0x13B3B0], pidl2=[0x93130], w=[0x136F14]
 util_file.cpp(462) : ufile::GetIconLL(file=[0x1ED54F0], index=[0x0], dwFlags=[0x0], outFlags=[0x0]);  Locals: cxy=[0x10], filew=[0x142034], hi=[0x0], shfi=[0xCCCCCCCC], pidl=[0x0], su=[0x142034], shflags=[0x0], sil=[0xCCCCCCCC], erm=[0x8005]
 util_file.cpp(506) : ufile::GetIcon(file=[0x1ED54F0], index=[0x0], flags=[0x0], outFlags=[0x0]);  Locals: __s=[0x0], s1=[0x4B1FD9C], s2=[0x4B1FDAC]
 util.cpp(230) : GetFileIcon(file=[0x1EAA9C8], index=[0x0], flags=[0x0])
 dll_r.cpp(216) : CRun::f_dll4_r();  Locals: fa=[0x1EAA9A4], fct=[0x4B1FEA0]
 var_r.cpp(19) : CRun::equali_r();  Locals: v=[0x4B1FF20]
 runflow_r.cpp(478) : CRun::Run();  Locals: nl=[0x2]
 run_r.cpp(88) : Thread2(id=[0x2195], cd=[0x1EAA940], locbase=[0x4B1FF20], rc=[0x0], nargsused=[0x0], speed=[0x64]);  Locals: run=[0x1EAA9C8], success=[0x0], t=[0x1ED5EF0]
 run_r.cpp(238) : ThreadR(v=[0x0]);  Locals: t=[0x1ED5EF0], locbase=[0x4B1FF20], speed=[0x64], cd=[0x1EAA940], sepprocess=[0xB1FF2000], nargsused=[0x0], rc=[0x0], id=[0x2195]
 ver.cpp(436) : CTls::__beginthread_proc(v=[0x1ED5EF0]);  Locals: r=[0xCCCCCCCC], ptls=[0x1ED5EF0]
 debug: exception in ufile::GetIconSpec.
