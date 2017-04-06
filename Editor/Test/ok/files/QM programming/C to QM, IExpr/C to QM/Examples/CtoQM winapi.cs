 This macro was used to create WINAPI reference file.
 Uses Vista SDK header files.
 Uses VC6 CRT header files for msvcrt.dll.


lpstr incl=
 $program files$\Microsoft SDKs\Windows\v6.0A\Include
 $program files$\Microsoft Visual Studio\VC98\Include

lpstr pp=
 WINVER 0x0502
 _WIN32_WINNT 0x0502
 NTDDI_VERSION 0x05020000
 _WIN32_IE 0x0603

out
ConvertCtoQM "$qm$\ctoqm\precompile\windows.cpp" "$qm$\winapi.txt" incl pp 4|8 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt"

 dll 6125, type 2334, interface 511, def 27776, guid 887, typedef 4273, callback 264, added 34436

out "<>Tasks:  <open>Check ref</open>"
