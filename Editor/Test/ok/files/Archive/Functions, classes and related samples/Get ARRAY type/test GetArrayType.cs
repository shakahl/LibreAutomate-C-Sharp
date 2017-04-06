out
ARRAY(int) aInt.create
ARRAY(byte) aByte.create
ARRAY(word) aWord.create
ARRAY(long) aLong.create
ARRAY(double) aDouble.create
ARRAY(str) aStr.create
ARRAY(lpstr) aLpstr.create
ARRAY(BSTR) aBstr.create
ARRAY(VARIANT) aVariant.create
ARRAY(CURRENCY) aCurrency.create
ARRAY(FLOAT) aFloat.create
ARRAY(DECIMAL) aDecimal.create
ARRAY(DATE) aDate.create
ARRAY(RECT) aRect.create
ARRAY(Acc) aAcc.create
ARRAY(Dir) aDir.create
ARRAY(IXml) aIXml.create
ARRAY(IAccessible) aIAccessible.create
ARRAY(double*) aPtr.create
ARRAY(int) aNotCreated

int t=GetArrayType(aInt)
if(t=0) out "array not created or invalid"
else if(t<0) out "type unknown, size=%i" -t
else
	sel t
		case VT_I4; out "int or pointer"
		case VT_UI1 out "byte"
		case VT_I2 out "word"
		case VT_I8 out "long"
		case VT_R8 out "double"
		case VT_BSTR out "BSTR"
		case VT_VARIANT out "VARIANT"
		case VT_CY out "CURRENCY"
		case VT_R4 out "FLOAT"
		case VT_DECIMAL out "DECIMAL"
		case VT_DATE out "DATE"
		case VT_UNKNOWN out "IUnknown or an interface that does not have IDispatch"
		case VT_DISPATCH out "IDispatch or an interface that has IDispatch"
		case 256 out "str"
		case 257 out "lpstr"
		case 258 out "Acc"

SAFEARRAY** a=&aInt.psa
int i n=&aNotCreated-&aInt/4+1
for i 0 n
	out GetArrayType(a[i])
