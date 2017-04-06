 /exe
 act "ddddddddddddddd"
int w=win("Quick Macros - ok - [Macro2226]" "QM_Editor")
SendMessageW w WM_SETTEXT -1 @"Test ąčę[]"

 BEGIN PROJECT
 main_function  Macro2226
 exe_file  $my qm$\Macro2226.qmm
 flags  6
 guid  {272C96E1-957F-4944-9EEF-8982A9E67995}
 END PROJECT
