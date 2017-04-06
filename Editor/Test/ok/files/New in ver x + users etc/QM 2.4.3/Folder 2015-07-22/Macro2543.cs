 /exe

def ICONFILE ":5 $system$\shell32.dll,15"
__Hicon hi=GetFileIcon(ICONFILE)
 __Hicon hi=GetFileIcon(":5 $system$\shell32.dll,15")
out hi

 BEGIN PROJECT
 main_function  Macro2543
 exe_file  $my qm$\Macro2543.qmm
 flags  22
 guid  {832A2369-451A-414E-88CF-A76A89256687}
 END PROJECT
