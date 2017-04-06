 This macro was used to create WINAPI7 reference file.
 Uses Windows 7 SDK header files.
 Uses VC6 CRT header files for msvcrt.dll.


lpstr incl=
 $program files$\Microsoft SDKs\Windows\v7.0\Include
 $program files$\Microsoft Visual Studio\VC98\Include

out
ConvertCtoQM "$qm$\ctoqm\precompile\windows.cpp" "$qm$\winapi7.txt" incl "" 4|8|128 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "" "" "ref WINAPIV"

 dll 6648, type 2624, interface 598, def 30766, guid 1055, typedef 4693, callback 281, added 38170

out "<>Tasks:  <open>Subtract ref</open>,  <open>Check ref</open>."
