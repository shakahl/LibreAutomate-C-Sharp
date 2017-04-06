 /remap_keyboard
function nCode wParam KBDLLHOOKSTRUCT&h

 This function runs as a hook procedure, called by Windows on each key down and up event.


if(nCode!=HC_ACTION) goto gRet ;;normally always HC_ACTION
if(h.flags&LLKHF_INJECTED) goto gRet ;;sent by a macro, not by the user
 out h.vkCode; goto gRet

 call a function that returns a nonempty value if remapping needed for this key
VARIANT v=remap_keyboard_my_map(h)

if v.vt ;;remapping found
	 send the remap-key and "eat" the source-key
	int up=h.flags&LLKHF_UP
	opt keysync 1 ;;minimal synchronization; recommended in functions like this
	sel v.vt
		case VT_I4 ;;virtual-key code etc
		if(!up) key+ (v.lVal); else key- (v.lVal)
		
		case VT_BSTR ;;text
		if(!up) _s=v; key (_s)
		
	ret 1 ;;return 1 means "eat" the event, ie not pass to the foreground window as well as to other hooks

 gRet
ret CallNextHookEx(0 nCode wParam &h) ;;need this, otherwise other hooks (eg QM keyboard triggers) will not receive the event
