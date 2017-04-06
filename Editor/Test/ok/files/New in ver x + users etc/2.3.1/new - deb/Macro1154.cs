deb
Dir d
foreach(d "*" FE_Dir)
	str sPath=d.FileName(1)
	out sPath
	

 Call stack of thread Macro1154:
 str.cpp(1008) : CStr::FromW(s=[0x1F68DC], codepage=[0xFDE9], lenw=[0x8]);  Locals: st=[0x0], lena=[0xCCCCCCCC]
 str.h(324) : CStr::FromW(s=[0x1F68DC])
 str_ex.cpp(677) : CStr::GetFullPath(s=[0x34DF9E8]);  Locals: ss=[0x0], lens=[0x1], s1=[0x149C24], s2=[0x1F68DC]
 dll_r.cpp(216) : CRun::f_dll4_r();  Locals: fa=[0x32A94B0], fct=[0x21DF724]
 runflow_r.cpp(430) : CRun::Run();  Locals: nl=[0x10]
 udf_r.cpp(120) : CRun::f_method_r();  Locals: he=[0x15AC0100], run=[0x34DF9E8], nargs=[0x1], cd=[0x32A9368], i=[0x1]
 runstr_r.cpp(799) : CRun::string_r(s=[0x21DFEE4], noF=[0x0])
 runstr_r.cpp(752) : CRun::str_equal_r();  Locals: s=[0x21DFEE4]
 runflow_r.cpp(485) : CRun::Runi(first=[0x2], last=[0x4])
 udf_r.cpp(156) : CRun::foreach_udf_r(first=[0x2], last=[0x4]);  Locals: he=[0x159E0200], run=[0x1], nargs=[0x2], cd=[0x34DBF58], i=[0x1]
 runflow_r.cpp(170) : CRun::foreach_r();  Locals: ss=[0x4E7CAF], item=[0xFFFFFFFF], coll=[0x2C0], s=[0x1], i=[0x0], a=[0xFFFFFFFF], last=[0x4], aa=[0x0], first=[0x2]
 runflow_r.cpp(430) : CRun::Run();  Locals: nl=[0x4]
 run_r.cpp(89) : Thread2(id=[0x1985], cd=[0x3340A28], locbase=[0x21DFB74], rc=[0x0], nargsused=[0x0], speed=[0x64]);  Locals: run=[0x1], success=[0x0], t=[0x327ACF0]
 run_r.cpp(237) : ThreadR(v=[0x0]);  Locals: t=[0x327ACF0], locbase=[0x21DFB74], speed=[0x64], cd=[0x3340A28], sepprocess=[0x1DFB7400], nargsused=[0x0], rc=[0x0], id=[0x1985]
 ver.cpp(361) : CTls::__beginthread_proc(v=[0x327ACF0]);  Locals: r=[0xCCCCCCCC], ptls=[0x327ACF0]
 debug: handled exception. Dump file created in C:\Users\G\AppData\Roaming\GinDi\Quick Macros\Debug\1176648652.
 Error (RT) in Dir.FileName:  Exception 0xC0000005. Access violation. Cannot read memory at 0x0. In qm.exe at 0x575FB0 (0x400000+0x175FB0).
