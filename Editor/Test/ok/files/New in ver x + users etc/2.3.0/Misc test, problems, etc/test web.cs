 /exe
int ie
 web "" 9 "" "" 0 ie
 zw ie
 web "" 1 "" "" 0 ie
 web "" 8 "" "" 0 ie
 web "" 8 "" "" 0
 web "" 8
 web ""
 web "http://www.google.lt"
 web "http://www.google.lt" 8
 web "http://www.google.lt" 1
 web "http://www.google.lt" 9
 web "http://www.google.com" 2
 web "http://www.google.com" 3
 web "http://www.google.com" 5
 web "http://www.google.com" 13
 web "http://www.google.com" 13|64
 web "" 8 "" "" _s; out _s
 web "www.google.com" 8 "" "" _s; out _s
 web "www.google.com" 9 "" "" _s; out _s

 web "www.google.com" 9 "" "http://www.google.lt"
 web "www.google.com" 8 "" "http://www.google.lt"
 web "www.google.com" 0 "" "http://www.google.lt"
 web "www.google.com" 1 "" "http://www.google.lt"

 int w1=win("Microsoft Document Explorer" "wndclass_desked_gsk")
 web "www.google.com" 1 w1

 BEGIN PROJECT
 main_function  Macro232
 exe_file  $my qm$\Macro232.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {36DC9E17-A4F7-4F90-B4D5-3B432241BC11}
 END PROJECT
