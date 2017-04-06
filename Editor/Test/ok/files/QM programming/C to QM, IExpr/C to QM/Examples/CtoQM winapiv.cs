 This macro was used to create WINAPIV reference file.
 Uses Vista SDK header files.
 Uses VC6 CRT header files for msvcrt.dll.


lpstr incl=
 $program files$\Microsoft SDKs\Windows\v6.0A\Include
 $program files$\Microsoft Visual Studio\VC98\Include

lpstr pp=
 WINVER 0x0600
 _WIN32_WINNT 0x0600
 NTDDI_VERSION 0x06000000
 _WIN32_IE 0x0700

out
ConvertCtoQM "$qm$\ctoqm\precompile\windows.cpp" "$qm$\winapiv.txt" incl pp 4|8|128 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt"

 dll 6452, type 2449, interface 568, def 28982, guid 944, typedef 4465, callback 271, added 36053

out "<>Tasks:  <open>Subtract ref</open>,  <open>Check ref</open>"
