ARRAY(POINT) p.create(10)
 ARRAY(BSTR) p.create(10)

rep 1000000
 rep 2
	VARIANT v=p
	 outx v.vt
	 v.vt|=VT_RECORD
	 outx VariantClear(&v)
	
	ARRAY(RECT) r.create(10)
	VARIANT vv.attach(r)
	 outx vv.vt

 ARRAY(POINT) pp=v
 out pp.len
