 /Macro1568
function action ;;action: 0 encode, 1 decode

int i n(this.len) c e b
if(!n) ret

sel action
	case 0
	for i 0 n
		c=this[i]
		if(c>=32) e=c>126 or c='='
		else if(c=13) if(this[i+1]=10) i+1; continue; else e=1
		else e=c!=9
		
		if !e
			if(c>32) continue
			if(i<n-1 and !(this[i+1]=13 and this[i+2]=10)) continue
		
		if(b<i) _s.geta(this b i-b)
		b=i+1
		_s.formata("=%02X" c)

if(b<n) _s.geta(this b n-b)
_s.Swap(this)

 not finished (does not wrap lines)
