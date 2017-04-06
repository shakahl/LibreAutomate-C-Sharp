 ARRAY(int) a=VbsEval("Array(Array(1, 1), Array(2, 2), Array(3, 4))")
VARIANT v=VbsEval("Array(Array(1, 1), Array(2, 2), Array(3, 4))")
 out "0x%X" v.vt
int i=v.parray[0].
out "0x%X" i
