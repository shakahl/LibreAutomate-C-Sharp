function ARRAY(Acc)&array

 Now don't need. We have GetChildObjects.

 Gets all direct child objects.

 array - variable that receives child objects.

 Added in: QM 2.3.3.


if(!a) end ERR_INIT
array=0
int i n
n=ChildCount; if(!n) ret
IEnumVARIANT en=+a; err
if en
	ARRAY(VARIANT) av.create(n)
	en.Reset; n=en.Next(n &av[0])
	array.create(n)
	for i 0 n
		VARIANT& v=av[i]
		Acc& r=array[i]
		sel v.vt
			case VT_DISPATCH
			r.a=v.pdispVal
			
			case VT_I4
			r.a=a.Child(v); err
			if(!r.a) r.a=a; r.elem=v.lVal
			
			case else end ERR_FAILED
else
	array.create(n)
	VARIANT vv.vt=VT_I4
	for i 0 n
		&r=array[i]
		vv.lVal=i+1
		r.a=a.Child(vv); err
		if(!r.a) r.a=a; r.elem=vv.lVal

err+
