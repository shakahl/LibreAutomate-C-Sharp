out
str h="Q:\Downloads\accessbridge2_0_2\src\include\AccessBridgeCalls.h"
str txt="$my qm$\jab.txt"
 str inclDir="$program files$\Java\jdk1.7.0_21\include"
str inclDir="Q:\Downloads\accessbridge2_0_2\src\include"
str preproc="QMCONVERT[]ACCESSBRIDGE_ARCH_LEGACY"
ConvertCtoQM h txt inclDir preproc 0 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"

 run "qm.exe" F"''{txt}''"

 note: QM crashes if Debug config. It seems like stack overflow in pcre_compile.
