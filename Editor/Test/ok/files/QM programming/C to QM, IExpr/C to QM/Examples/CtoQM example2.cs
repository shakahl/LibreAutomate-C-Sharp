 Converts C declarations in somefile.h to QM and saves to somefile.txt.
 This example can be used if somefile.h does not #include system header files or you don't want to add declarations from them.
 If it #include's them, you must remove or comment out all #include lines.
 If somefile.h uses something from system header files, that data will be taken from winapiv_pch.txt (it contains precompiled declarations from system header files).


out
ConvertCtoQM "$qm$\somefile.h" "$qm$\somefile.txt" "" "" 0 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
