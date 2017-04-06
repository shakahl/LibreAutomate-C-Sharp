 This macro was used to create WINAPI2 (more declarations than in WINAPI).
 Uses Windows 7 SDK header files.
 Uses VC6 CRT header files for msvcrt.dll.
 Uses preprocessor definitions for Win7/IE8.


lpstr incl=
 $program files$\Microsoft SDKs\Windows\v7.0\Include
 $program files$\Microsoft Visual Studio\VC98\Include

lpstr pp=
 CH_AZ

out
ConvertCtoQM "$qm$\ctoqm\precompile\windows.cpp" "$qm$\winapi2.txt" incl pp 0 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt"

 dll 17615, type 10512, interface 5808, def 143300, guid 10949, typedef 18970, callback 2147, added 204613
