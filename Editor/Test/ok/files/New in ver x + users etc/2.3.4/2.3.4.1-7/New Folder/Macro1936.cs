 /exe

 #exe addfile "$qm$\il_de.xml" 100
#exe addfile "$qm$\il_de.xml" "name" "type"

str s
 if(!ExeGetResourceData(0 100 &s)) ret
if(!ExeGetResourceData("type" "name" &s)) ret
 if(!ExeGetResourceData(L"type" L"name" &s)) ret
out s

 if(!ExeExtractFile(100 "$temp$\testexe\test.txt")) ret
if(!ExeExtractFile("name" "$temp$\testexe\test.txt" 0 "type")) ret
 if(!ExeExtractFile(L"name" "$temp$\testexe\test.txt" 0 L"type")) ret
run "$temp$\testexe\test.txt"

 out F"0x{_hinst} 0x{GetExeResHandle}"

 BEGIN PROJECT
 main_function  Macro1936
 exe_file  $my qm$\Macro1936.qmm
 flags  6
 guid  {1B147BE8-E97C-4216-B2C5-C1A5ECF1DD2D}
 END PROJECT
