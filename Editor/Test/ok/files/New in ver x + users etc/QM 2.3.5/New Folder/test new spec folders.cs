/exe
 out _s.expandpath("$my qm$")
 GetFullPath "$my qm$\..\" &_s; out _s
 out _s.expandpath("%downloads%")
 out _s.expandpath(_s 2)

 out PathSkipRoot("c:\folder")
 out PathSkipRoot("\\server\comp\folder")
 out PathSkipRoot("\\?\UNC\server\share\folder")

out
out _s.expandpath("$qm$\folder")
out _s.expandpath(_s 2)
out _s.expandpath("$documents$\folder")
out _s.expandpath(_s 2)
out _s.expandpath("$appdata$\folder")
out _s.expandpath(_s 2)
out _s.expandpath("$cookies$\folder")
out _s.expandpath(_s 2)
out _s.expandpath("$drive$\folder")
out _s.expandpath(_s 2)

 BEGIN PROJECT
 main_function  Macro2072
 exe_file  $my qm$\Macro2072.qmm
 flags  6
 guid  {DEFE1D3A-B539-4C11-85F9-25DA5522652B}
 END PROJECT
