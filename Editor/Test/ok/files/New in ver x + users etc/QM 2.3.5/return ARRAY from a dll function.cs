dll "qm.exe" !myfunc ARRAY(BSTR)a

ARRAY(BSTR) b.create(5)
out myfunc(b)
ARRAY(str) a.create(b.len); for(_i 0 a.len) a[_i]=b[_i] ;;tip: create function for this
out a
