 This macro was used to create WINAPI2 (more declarations than in WINAPI).
 Uses Vista SDK header files.
 Uses VC6 CRT header files for msvcrt.dll.
 Uses preprocessor definitions for Vista/IE7.


lpstr incl=
 $program files$\Microsoft SDKs\Windows\v6.0A\Include
 $program files$\Microsoft Visual Studio\VC98\Include

lpstr pp=
 CH_AZ

out
ConvertCtoQM "$qm$\ctoqm\precompile\windows.cpp" "$qm$\winapi2.txt" incl pp 0 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt"

 dll 15835, type 8469, interface 4300, def 117523, guid 9016, typedef 16004, callback 1883, added 169375
 dll 15835, type 8466, interface 4300, def 117532, guid 9016, typedef 16004, callback 1883, added 169382
