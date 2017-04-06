 \

 QM runs this exe on unhandled exception.

0.1
#compile "__CVisualStudio"
CVisualStudio x
x.AttachToQM

int w3=win("Quick Macros" "#32770" "" 1 "cClass=Static[]cText=Stopped working")
if(!w3) ret
but IDNO w3

 this did not work, never mind
 w3=wait(5 WV win("Visual Studio" "#32770" "" 0 "cClass=Static[]cText=Unhandled exception"))
 if(w3) but "&Break" w3

 BEGIN PROJECT
 main_function  attach_debugger_to_qm
 exe_file  q:\app\app_plus\attach_debugger_to_qm.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {C3F91C3D-EC25-4A5F-9F1D-E4EA7A3C37E2}
 END PROJECT
