 /Selenium help
function# $json $name str&value [valueType] ;;valueType: 0 any, 1 string or null, 2 number, 3 bool, 4 object, 5 array

 Finds name in JSON object string, and gets its value.
 Gets first found "name":, regardless where it is.
 If the value is:
   string: unescapes and returns 1.
   number: returns 2.
   bool: returns 3.
   object: removes {} and returns 4.
   array: removes [] and returns 5.
   null: value.all and returns -1.
 Returns 0 if fails or if the value type does not match valueType.
 No errors.


int i j
lpstr v a
sel(valueType) case 0 a="."; case 1 a="[''n]"; case 2 a="[0-9\-]"; case 3 a="[tf]"; case 4 a="\{"; case 5 a="\["; case else ret
i=findrx(json F"''{name}''\s*:\s*{a}" 0 0 j); if(i<0) ret
v=json+i+j

sel v[-1]
	case 34
	
	case '{'
	a=__Brac2(v '{' '}' 0x4000); if(a[0]!'}') ret
	value.left(v a-v)
	ret 4
	
	case '['
	a=__Brac2(v '[' ']' 0x4000); if(a[0]!']') ret
	value.left(v a-v)
	ret 5
	
	case 'n'
	value.all
	ret -1
	
	case 't'
	value="true"
	ret 3
	
	case 'f'
	value="false"
	ret 3
	
	case else
	if(findrx(v-1 "^[0-9\.\-+Ee]+" 0 0 value)) ret
	ret 2

 string
value=""
rep
	i=findcs(v "''\"); if(i<0) ret
	if(v[i]=34) break
	value.geta(v 0 i)
	v+i+2
	sel v[-1]
		case 34 a="''"
		case '\' a="\"
		case '/' a="/"
		case 'n' a="[10]"
		case 'r' a="[13]"
		case 't' a="[9]"
		case 'b' a="[8]"
		case 'f' a="[12]"
		case 'u'
		_s.left(v 4); v+4
		long k=__Val(_s a 12); if(a-_s!=4) ret
		word* w=&k; a=_s.ansi(w)
		
		case else ret
	value+a
value.geta(v 0 i)
ret 1
