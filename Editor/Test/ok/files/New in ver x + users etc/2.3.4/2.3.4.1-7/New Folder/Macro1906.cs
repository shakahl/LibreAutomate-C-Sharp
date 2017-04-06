ShellExecFromExplorer("$system$\notepad.exe")
 ShellExecFromExplorer("$system$\msconfig.exe") ;;runs High without consent
 ShellExecFromExplorer("Q:\My QM\admin_exe.exe")

 run "Q:\My QM\admin_exe.exe"

 spe
 _s.expandpath("$system$\notepad.exe")
 Q &q
  ShellExecuteW 0 0 @_s 0 0 1
 run _s
 Q &qq
 outq
