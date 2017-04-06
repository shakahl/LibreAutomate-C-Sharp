 /exe

str cur=":1 $qm$\cross_red.cur"
 out cur
str bmp=":1 $qm$\il_icons.bmp"
str ico=":3 $qm$\copy.ico"
str ico2=":2 $qm$\paste.ico"
str xml=":2 $qm$\toolbars.xml"
str exeicon=":33 $system$\notepad.exe,0"
str exeicon3=":5 $qm$\qm.exe,0"
str exeicon4=":6 $qm$\qmcl.exe,0"
str exeicon6=":7 $system$\shell32.dll,5"
out 1
 str exe=":1 $qm$\qmcl.exe"
 str exeicon2=":4 $system$\notepad.exe"

 BEGIN PROJECT
 main_function  make exe - auto add files test extensions
 exe_file  $my qm$\bad.qmm
 res  $qm$\Release\app.res
 flags  22
 guid  {A06721D5-249E-4A9A-A784-D2D76C7A5512}
 END PROJECT
