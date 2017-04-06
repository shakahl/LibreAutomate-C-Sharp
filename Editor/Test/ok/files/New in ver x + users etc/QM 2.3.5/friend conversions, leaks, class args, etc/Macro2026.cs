out
 BSTR b="test"
 VARIANT v=&b
 outx v.vt
 VARIANT vv
 out VariantChangeType(&vv &v 0 VT_BSTR)
 outx vv.vt
 out vv

 BSTR b="test"
 VARIANT v=b
 outx v.vt
 VARIANT vv
 out VariantChangeType(&vv &v 0 VT_BSTR|VT_BYREF)
 outx vv.vt
 out vv

 ARRAY(BSTR) a="test"
 VARIANT v=a
 outx v.vt
 VARIANT vv
 out VariantChangeType(&vv &v 0x409 VT_BSTR|VT_ARRAY)
 outx vv.vt
 out vv
