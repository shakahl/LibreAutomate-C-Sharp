 this also works in exe and in macros running in separate process

int w
#if EXE
w=FindWindow("QM_Editor" 0)
if(w=0) mes "no window"; ret
#else
w=_hwndqm
#endif

mes F"active={w=win}, visible={!hid(w)}, minimized={min(w)}, enabled={IsWindowEnabled(w)}"

 BEGIN PROJECT
 main_function  Macro1721
 exe_file  $my qm$\Macro1721.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {FB14125B-EEC0-408B-9709-8DF1D373A6C4}
 END PROJECT
