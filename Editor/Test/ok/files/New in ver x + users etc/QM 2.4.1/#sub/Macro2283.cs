/exe
 #exe addfile "$my qm$\test.bmp" 3
 #exe addfile "$my qm$\test3.bmp" 3

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:hEABD168C" child("VA Find References Results" "GenericPane" w) 0 1|2 ;; 'VA Find References Results'

 #exe addfile "resource:<Macro2283>image:hEABD168C" "hEABD168C" "image"
 #exe addfile "image:hEABD168C" "hEABD168C" "image"
sub.Test

#sub Test
 #exe addfile "image:hEABD168C" "hEABD168C" "image"
 _s.getfile("resource:<Macro2283>image:hEABD168C"); out _s.len

int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
scan "image:hEABD168C" child("VA Find References Results" "GenericPane" w) 0 1|2 ;; 'VA Find References Results'

 BEGIN PROJECT
 main_function  Macro2283
 exe_file  $my qm$\Macro2283.qmm
 flags  7
 guid  {10B16AC7-AAFF-4321-926D-1A149054742C}
 END PROJECT
