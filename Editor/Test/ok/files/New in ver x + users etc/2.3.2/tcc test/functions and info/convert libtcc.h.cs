out
str h="$qm$\tcc\project\libtcc.h"
str t="$desktop$\tcc.txt"
lpstr incl=
 $program files$\Microsoft Visual Studio\VC98\Include
ConvertCtoQM h t incl "_WIN32 1" 32

str s.getfile(t)
s.replacerx("^dll \?\?\?" "dll tcc\tcc" 8)
s.setfile(t)

run t
1
del- t
