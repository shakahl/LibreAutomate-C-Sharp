 /exe
out 1
#exe addfunction "TcpSocket_ServerThread"
#compile "__TcpSocket"
TcpSocket x.ServerStart(5032 &test_TcpSocket_func)
mes 1

 BEGIN PROJECT
 main_function  Atest
 exe_file  $my qm$\Atest.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  7
 guid  {9D1F81E1-2078-436C-A420-6516284E8E29}
 END PROJECT
