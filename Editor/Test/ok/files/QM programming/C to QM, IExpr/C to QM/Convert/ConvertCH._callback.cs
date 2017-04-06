 /CtoQM
function# $s

 Converts callback function (typedef).

int db
str q ft comm
ARRAY(str) a

if(findrx(s "^(?:struct |enum |union |_struct_from_class )?(\w+)(\**)(\(\*?)? ?(\w+)(?(3)\)|)\((.*?)\);$" 0 0 a)<0)
	 out s
	ret 71
 out "%s %s %s %s" a[1] a[2] a[4] a[5]
str& name=a[4]
AddToMap(m_mtd name "#")

ConvType(a[1] a[2] ft 1 name)
if(ft.len and __iscsym(ft[0])) ft.rtrim("'"); ft-"'"
if(m_crt) ft-"[c]"
q.format("function%s" ft)
 
if(!FuncArgs(a[5] q 0 comm name)) ret 72

AddToMap(m_mfcb name q comm)

 db=1
if db
	out "%s %s" name q
