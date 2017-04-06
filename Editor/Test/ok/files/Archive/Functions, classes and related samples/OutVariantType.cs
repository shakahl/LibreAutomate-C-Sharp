 /
function VARIANT&v

 Shows VARIANT variable type in QM output: VT_ constant, matching QM type, and for some types a note.


if(v.vt&0x9F00) out "unsupported flags in vt"; ret

lpstr csv=
 VT_EMPTY,,the VARIANT is not set
 VT_NULL,,means there is no data
 VT_I2,word,signed
 VT_I4,int
 VT_R4,FLOAT
 VT_R8,double
 VT_CY,CURRENCY
 VT_DATE,DATE
 VT_BSTR,str
 VT_DISPATCH,IDispatch
 VT_ERROR,int,used for omitted optional arguments
 VT_BOOL,word,"QM does not have a bool type. Physically it is 2-byte integer, like word. Value 0 is FALSE, 0xFFFF or other is TRUE."
 VT_VARIANT,VARIANT
 VT_UNKNOWN,IUnknown
 VT_DECIMAL,DECIMAL
;
 VT_I2,byte,signed
 VT_UI1,byte
 VT_UI2,word
 VT_UI4,int,unsigned
 VT_I8,long
 VT_UI8,long,unsigned
 VT_INT,int
 VT_UINT,int,unsigned

ICsv x._create
x.FromString(csv)

int i=v.vt&0xff
if(i>=x.RowCount or i=15) out "unknown type"; ret

str u=x.Cell(i 0)
str q=x.Cell(i 1)
str n=x.Cell(i 2)
if(v.vt&0x2000) q-"ARRAY("; q+")"; u+"|VT_ARRAY"
if(v.vt&0x4000) q+"*"; u+"|VT_BYREF"

str r=F"VARIANT type: {u}[]QM type: {q}"
if(n.len) r+F"[]note: {n}"

out r
