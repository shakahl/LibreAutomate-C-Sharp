 Function C_header_files converts C #define to QM def constants.
 This sample converts windows.h and all files it includes.
 All converted files will be placed into "include" folder on Desktop.
 For more info look in function C_header_files.

mkdir("$Desktop$\include")
str files.flags=3
 C_header_files("windows.h" "c:\devstudio\vc98\include" 1 &files)
 CH_Convert("commctrl.h" "c:\devstudio\vc98\include")
CH_Convert("curl.h" "C:\Documents and Settings\a\Desktop\curl-7.10.6\include\curl")
 out files