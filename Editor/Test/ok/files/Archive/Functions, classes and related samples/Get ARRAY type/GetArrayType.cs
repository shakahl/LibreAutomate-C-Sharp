 /
function# SAFEARRAY*a ;;Returns: str 256, lpstr 257, Acc 258, int VT_I4, byte VT_UI1, word VT_I2, long VT_I8, double VT_R8, other VT_, unknown -size, empty 0.

 Returns array type, if possible.
 If cannot get type, returns -size.
 Returns 0 if array not created or somehow invalid.

 For some types returns one of VT_ constants. Some listed above.
 For some other types returns >=256. See above. You can edit this function to add more known types.
 For COM interfaces returns either VT_UNKNOWN or VT_DISPATCH.
 For pointers returns VT_I4 (int).


word vt; IRecordInfo ri
if !SafeArrayGetVartype(a &vt)
	if(vt>255) ret iif(vt&0xff00=VT_BYREF VT_I4 0)
	if(vt!=VT_RECORD) ret vt
	if !SafeArrayGetRecordInfo(a &ri)
		ARRAY(int)+ ___gat; int i
		if !___gat.len
			lock
			if !___gat.len
				ARRAY(str) a0.create
				ARRAY(lpstr) a1.create
				ARRAY(Acc) a2.create
				 Here you can add more array types like the above 3 lines. Then restart QM or reopen file.
				 Add only composite types, because simple types like RECT don't have IRecordInfo. Simple types have only size.
				 Composite types are types and classes that have a destructor or contain str, BSTR, VARIANT, ARRAY, interface, another composite type.
				
				SAFEARRAY** k=&a0.psa
				___gat.create(&k-&a0/4)
				for(i 0 ___gat.len) SafeArrayGetRecordInfo(k[i] +&___gat[i])
		
		for(i 0 ___gat.len) if(ri=___gat[i]) ret 256+i

ret -SafeArrayGetElemsize(a)
