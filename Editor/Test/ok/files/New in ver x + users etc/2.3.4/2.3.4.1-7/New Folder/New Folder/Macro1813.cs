/exe
out
int w=win("Options" "#32770")
POINT p.x=26; p.y=190
POINT ps=p; ClientToScreen w &ps
Q &q
int c=RealChildWindowFromPoint(w, p)
Q &qq
int ht=SendMessageW(c WM_NCHITTEST 0 MakeInt(p.x p.y))
Q &qqq
outq
outw c
out ht


 BEGIN PROJECT
 main_function  Macro1813
 exe_file  $my qm$\Macro1813.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {A8810ED5-7278-4308-AF21-E418C5183C65}
 END PROJECT
