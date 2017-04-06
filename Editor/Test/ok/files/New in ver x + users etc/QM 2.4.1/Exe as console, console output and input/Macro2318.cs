 /exe

 ExeConsoleWrite "default ąčę Σ"
 ExeConsoleWrite "stderr ąčę Σ"
 ExeConsoleWrite "no redirect ąčę Σ" 2
 ExeConsoleWrite "no " 4; ExeConsoleWrite "newline" 4

 ExeConsoleWrite "Type something and press Enter"
 str s
 ExeConsoleRead s
 ExeConsoleWrite F"s=''{s}''"
 ExeConsoleWrite F"s=''{s}''" 2

ExeConsoleRedirectQmOutput
out "default ąčę Σ"
 act "ffffffffffffff"

 ExeConsoleWrite "ExeConsoleWrite"
 puts "puts"
 printf "pri[10]ntf[10]"
 _cputs "_cputs"
 WriteFile GetStdHandle(STD_OUTPUT_HANDLE) "Write[10]File[10]" 11 &_i 0

 ExeConsoleWrite "default ąčę Σ"
 ExeConsoleWrite "no redirect ąčę Σ" 2
 ExeConsoleWrite "no newline ąčę Σ" 2
 ExeConsoleWrite "no redirect/newline ąčę Σ" 2|4
 ExeConsoleWrite "stderr ąčę Σ" 1
 ExeConsoleWrite "stderr no unic" 1

 int h=GetStdHandle(STD_OUTPUT_HANDLE)
 WriteConsole h "WriteConso[]" 12 &_i 0
 ExeConsoleWrite "ExeConsoleWrite"
 WriteFile h "WriteFile_[]" 12 &_i 0
  
 PF
 rep(50) ExeConsoleWrite "ExeConsoleWrite"
 PN
 rep(50) ExeConsoleWrite "ExeConsoleWrite 12345" 2
 PN
 rep(50) ExeConsoleWrite "ExeConsoleWrite 12345"
 PN
 PO

2

 BEGIN PROJECT
 main_function  Macro2318
 exe_file  $my qm$\Macro2318.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {B066418D-119F-4A76-BE90-6BBF00E6F470}
 END PROJECT

#ret
"q:\my qm\Macro2318.exe"
