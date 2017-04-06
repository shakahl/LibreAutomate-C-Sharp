 \
function# QMTHREAD&t
 out t.qmitemid

if(mes("end thread %s?" "" "YN!" _s.getmacro(t.qmitemid 1))='Y') ret 1
