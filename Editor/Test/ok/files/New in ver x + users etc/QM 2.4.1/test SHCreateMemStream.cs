 /exe
__Stream is
mes 1
_s.all(1024*1024 2)
mes 2
 is.CreateOnHglobal(_s _s.len)
is.is=SHCreateMemStream(_s _s.len) ;;tested: copies _s, like CreateOnHglobal
_i=is.is
mes _i

 BEGIN PROJECT
 main_function  Macro2197
 exe_file  $my qm$\Macro2197.qmm
 flags  6
 guid  {BC21F15B-B9BA-454C-8E50-05BCEFA1EC2A}
 END PROJECT
