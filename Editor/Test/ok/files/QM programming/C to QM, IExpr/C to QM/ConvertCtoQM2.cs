 /
function str&s

 Converts one or more C declarations to QM.
 Similar to ConvertCtoQM, but here you use variable instead of files.
 These files must be in QM folder: winapiqmaz_fdn.txt, winapiqmaz_fan.txt, winapiv_pch.txt.

 s - variable that contains C declarations. They are converted in place.


str sf1="$temp$\ctoqm.h"
str sf2="$temp$\ctoqm.txt"
s.setfile(sf1)
ConvertCtoQM sf1 sf2 "" "" 16 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
s.getfile(sf2)
