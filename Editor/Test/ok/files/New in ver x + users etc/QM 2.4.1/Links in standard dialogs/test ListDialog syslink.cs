/exe
out ListDialog("one[]two" F"Text normal")
str sl=F"<>Test <a id=''200''>id 200</a>[]Test <a href=''{&syslink_click} param''>callback</a>"
out ListDialog("one[]two" sl)
 out ListDialog("" sl)

 BEGIN PROJECT
 main_function  test ListDialog syslink
 exe_file  $my qm$\Macro2256.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {91DED984-7178-4093-89BC-61DF4ECB82D3}
 END PROJECT
 manifest  
