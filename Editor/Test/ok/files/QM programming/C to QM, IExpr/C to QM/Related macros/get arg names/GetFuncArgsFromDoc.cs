 Some dll function declarations in C header files don't have argument names.
 ConvertCtoQM writes names of these functions to dest_file_fan_missing_crt.txt and dest_file_fan_missing_win.txt.
 This macro opens these files, extracts argument names from local MSDN Library (it must be running), and saves to _winapiqmaz_fan.txt. Then you can use _winapiqmaz_fan.txt with ConvertCtoQM.

 MSDN Library should have default filter "Platform SDK" and custom filter "C Run-Time Libraries (CRT)" whose filter string is "("Technology"="CRT")".

out
str s
GetFuncArgsFromDoc2 "$qm$\winapiqmaz_fan_missing_crt.txt" s
GetFuncArgsFromDoc2 "$qm$\winapiqmaz_fan_missing_win.txt" s
s.setfile("$qm$\_winapiqmaz_fan.txt")
