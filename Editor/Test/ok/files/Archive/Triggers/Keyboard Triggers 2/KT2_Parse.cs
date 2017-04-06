 /
function# $ts &mod &vk &flags forctrl

 def MOD_ALT 1
 def MOD_CONTROL 2
 def MOD_SHIFT 4
 def MOD_WIN 8

if(!len(ts)) ret
mod=0
vk=0
flags=0
rep
	sel ts[0]
		case 'S' mod|iif(forctrl 1 4)
		case 'C' mod|2
		case 'A' mod|iif(forctrl 4 1)
		case 'W' if(forctrl) flags|1; else mod|8
		case else break
	ts+1

lpstr fs=ts+QmKeyCodeToVK(ts &vk)
#opt nowarnings 1
if(fs=ts) ret
if(fs[0]=32) flags|val(fs)
ret 1
