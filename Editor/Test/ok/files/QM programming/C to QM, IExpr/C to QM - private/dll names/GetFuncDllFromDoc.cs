 ConvertCtoQM cannot get dll names of some dll functions. It writes names of these functions to dest_file_fdn_missing.txt.
 This macro opens the file, extracts dll names from local MSDN Library (it must be running), and saves to _winapiqmaz_fdn.txt. Then you can use _winapiqmaz_fdn.txt with ConvertCtoQM.
 Alternatively, use CH_GetDllNames (preferred).

 MSDN Library should have default filter "Platform SDK".

out
str s
GetFuncDllFromDoc2 "$qm$\winapiqmaz_fdn_missing.txt" s
s.setfile("$qm$\_winapiqmaz_fdn.txt")
