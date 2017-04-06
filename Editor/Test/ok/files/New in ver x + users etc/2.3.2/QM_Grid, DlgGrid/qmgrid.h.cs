out
str h="$qm$\qmgrid\grid.h"
str t="$desktop$\grid.txt"
ConvertCtoQM h t "" "" 4 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
 1
 run t
str s.getfile(t)
s.setmacro("__qm_grid_api")
