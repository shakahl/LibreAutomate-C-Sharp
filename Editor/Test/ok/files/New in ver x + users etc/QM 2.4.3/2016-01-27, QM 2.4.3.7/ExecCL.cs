 Executes QM macro text specified in command line, for example in Command Prompt (cmd.exe).

 Use this prefix to the macro text: qmcl.exe MS "ExecCL" C 
 For new lines use " ; ".

 EXAMPLE (in cmd.exe)
 qmcl.exe MS "ExecCL" C run "c:" ; run "$Program Files$\Internet Explorer\iexplore.exe" ; run "$Program Files$\Windows NT\Accessories\wordpad.exe"


str s=_command
s.findreplace(" ; " "[]")
s-"spe -1[]"
 out s
wait 0 H RunTextAsFunction(s)
