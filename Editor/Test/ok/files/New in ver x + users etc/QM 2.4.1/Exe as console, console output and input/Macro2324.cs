 /exe
ExeConsoleRedirectQmOutput ;;redirect out etc to ExeConsoleWrite
 SetConsoleTitle "QM test console"
 out "act"
 2; ret
 act "fffffffffffff"

ARRAY(str) a; int i
ExeParseCommandLine _command a
for(i 0 a.len) ExeConsoleWrite a[i]

ExeConsoleWrite "Type something and press Enter" 2
ExeConsoleWrite "stderr" 1
str s
ExeConsoleRead s
ExeConsoleWrite F"s=''{s}''"
2

 BEGIN PROJECT
 main_function  Macro2324
 exe_file  $my qm$\Macro2324.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  68
 guid  {B2F32A7C-587D-41FE-8594-B0954A9EC7C2}
 END PROJECT

#ret
"q:\my qm\Macro2324.exe" /one "two ooo" >"q:\my qm\test\test.txt"
