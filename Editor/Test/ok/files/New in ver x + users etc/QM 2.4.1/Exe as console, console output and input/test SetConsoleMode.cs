 /exe

int h=GetStdHandle(STD_INPUT_HANDLE)
GetConsoleMode h &_i
outx _i
 _i~ENABLE_ECHO_INPUT
 _i~ENABLE_LINE_INPUT
_i|ENABLE_QUICK_EDIT_MODE|ENABLE_EXTENDED_FLAGS
SetConsoleMode h _i
SetConsoleTitle "Title"

 AllocConsole
 ExeConsoleRedirectQmOutput
 out "Type something and press Enter"

ExeConsoleWrite "Type something and press Enter"
str s
ExeConsoleRead s
ExeConsoleWrite F"s=''{s}''"
2

 BEGIN PROJECT
 main_function  Macro2323
 exe_file  $my qm$\Macro2323.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  70
 guid  {614410D4-4D7B-405D-9DB9-399D95085511}
 END PROJECT
