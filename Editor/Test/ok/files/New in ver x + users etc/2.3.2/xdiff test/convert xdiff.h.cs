out
str h="$qm$\xdiff\xdiff.h"
str t="$desktop$\xdiff.txt"
lpstr incl=
 $program files$\Microsoft Visual Studio\VC98\Include
ConvertCtoQM h t incl "" 0 "" "" "$qm$\winapiv_pch.txt" "$qm$\xdiff"

out "<>Tasks:  <open>Check ref</open>."
