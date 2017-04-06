 /
function# $qmKeys int&mod int&vk [flags] ;;flags: 1 swap Shift with Alt in mod

 Converts QM key codes to modifier flags and virtual-key code.
 Reaturns hotkey part length of qmKeys. Returns 0 if qmKeys is invalid.

 qmKeys - a hotkey specified using QM key codes, for example "CSAk" for Ctrl+Shift+Alt+K.
   The order of C S A is not important. Spaces are ignored.
   qmKeys can contain more text, it is ignored.
 mod - receives modifier key flags: 1 Shift, 2 Ctrl, 4 Alt, 8 Win.
 vk - receives virtual-key code.


vk=0; mod=0
lpstr k=qmKeys; if(!k) ret
rep
	sel(k[0]) case 32; case 'S' mod|1; case 'C' mod|2; case 'A' mod|4; case 'W' mod|8; case else break
	k+1
_i=QmKeyCodeToVK(k &vk)
if(!_i or !vk) ret
if(flags&1) mod=(mod&1<<2)|(mod>>2)|(mod&2)
ret k+_i-qmKeys
