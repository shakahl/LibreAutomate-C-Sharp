 Converts C declarations in somefile.h to QM and saves to somefile.txt.
 This example can be used if somefile.h #include's system header files, and you have them, and want to add declaration from them too.


lpstr incl=
 $program files$\Microsoft SDKs\Windows\v7.0\Include
 $program files$\Microsoft Visual Studio\VC98\Include

out
ConvertCtoQM "$qm$\somefile.h" "$qm$\somefile.txt" incl "" 0 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt"
