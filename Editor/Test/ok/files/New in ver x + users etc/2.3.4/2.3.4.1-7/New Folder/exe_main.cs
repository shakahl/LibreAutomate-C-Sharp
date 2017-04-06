out
str sf="$qm$\Macro18-.exe"
cop- "$qm$\Macro18.exe" sf
goto gSend
str s.getfile(sf)

 IMAGE_DOS_HEADER* dos=+s
 IMAGE_NT_HEADERS* nt=s+dos.e_lfanew
 nt.FileHeader.NumberOfSections=2

int i j n
int* p

 p=s+0x424f0; *p=0x63698 ;;ReadProcessMemory
 p=s+0x42490; *p=0x63698 ;;Process32NextW

 exe_erase_api s 0x42390 0x42abc ;;all
 exe_erase_api s 0x423e4 0x423f8 ;;comctl
 exe_erase_api s 0x42390 0x42434 ;;before kernel
 exe_erase_api s 0x42678 0x42abc ;;after kernel

exe_erase s 0x600 0x37800 ;;.text
 s.fix(0x46e00) ;;.rsrc
 exe_erase s 0x48044

 ret
s.setfile(sf)
 gSend
str sf2="\\gintaras\myprojects\test\Macro18.exe"
del- sf2; err
2
cop sf sf2
