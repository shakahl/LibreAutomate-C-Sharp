out
str h="Q:\Downloads\portaudio\include\portaudio.h"
str t="$desktop$\portaudio.txt"
lpstr incl=
 $program files$\Microsoft Visual Studio\VC98\Include
ConvertCtoQM h t incl "" 0 "" "" "$qm$\winapiv_pch.txt" "$qm$\portaudio_x86"

out "<>Tasks:  <open>Check ref</open>."
