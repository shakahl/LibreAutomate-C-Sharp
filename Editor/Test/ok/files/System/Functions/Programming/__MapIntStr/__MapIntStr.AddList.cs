function $mapList ;;mapList: "int1 str1[]int2 str2[]..."

 Adds list of number/string pairs.


m_s=mapList
ARRAY(lpstr) as
tok(m_s as -1 "[]" 1)
a.create(as.len)
int i k
for i 0 a.len
	LPSTRINT& r=a[i]
	r.s=as[i]
	r.i=val(r.s 0 k)
	r.s+k; if(r.s[0]=32) r.s+1
