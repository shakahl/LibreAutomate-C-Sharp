out
str sf="$qm$\ext-.exe"; cop- "$qm$\ext.qme" sf
 str sf="$qm$\Macro18-.exe"; cop- "$qm$\Macro18.exe" sf
goto gSend
str s.getfile(sf)

int* p


 exe_erase_api s 0x60d44 0x60db4 ;;all API
exe_erase s 0x600 0x51c00 ;;.all code sections
exe_erase s 0x60440 0x63e00 ;;all data sections
 exe_erase s 0x51c00 0x60440 ;;.rdata
exe_erase s 0x52500 0x5c4b0 ;;.rdata all before IMPORT
exe_erase s 0x5f14a 0x60440 ;;EXPORT Names

 exe_erase s 0x5ce40 0x5ed30 ;;IMPORT Names
 exe_erase s 0x5ce40 0x5d150
 exe_erase s 0x5ce40 0x5d2e0
 exe_erase s 0x5ce40 0x5d220
exe_erase s 0x5ce40 0x5d1ec ;;KERNEL32
exe_erase s 0x5d200 0x5d950 ;;USER32
exe_erase s 0x5d960 0x5d9e0 ;;GDI32
exe_erase s 0x5d9f0 0x5da00 ;;ADVAPI32
exe_erase s 0x5da20 0x5dad0 ;;ole32
exe_erase s 0x5db00 0x5db30 ;;WINMM
exe_erase s 0x5db40 0x5dba0 ;;SHELL32
exe_erase s 0x5dbd0 0x5dc10 ;;COMCTL32
exe_erase s 0x5dc20 0x5de30 ;;MSVCRT
exe_erase s 0x5de50 0x5ec90 ;;MSVCRT+

 exe_erase_name s "KERNEL32" "-"
 exe_erase_name s "WINMM" "-"
 exe_erase_name s "WINMM" "-"
 exe_erase_name s "SHLWAPI" "-"
 exe_erase_name s "COMCTL32" "-"
 exe_erase_name s "SHELL32" "-"


 exe_erase s 0x118 0x11c ;;date stamp
 exe_erase s 0x60 0x64 ;;DOS program string
 exe_erase s 0x37400 0x51c00 ;;code sections except .text
 exe_erase s 0x37400 0x38e00 ;;-m
 exe_erase s 0x38e00 0x40400 ;;-z
 exe_erase s 0x60d44 0x60db4 ;;.data (delay-load)
 exe_erase_api s 0x60d44 0x60db4 ;;delay-load


s.setfile(sf)
 gSend
str sf2="\\gintaras\myprojects\test\ext.exe"
del- sf2; err
2
cop sf sf2
