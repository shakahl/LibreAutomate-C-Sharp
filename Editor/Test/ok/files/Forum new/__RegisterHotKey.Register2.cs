function! hWnd hkId $qmKeyCode

 Converts QM <help>key</help> codes qmKeyCode to mod/vk and calls <help>Register</help>.

 EXAMPLE
 __RegisterHotKey hk1.Register2(0 1 "CF10") ;;Ctrl+F10


int i k vk mod
rep
	i=QmKeyCodeToVK(qmKeyCode k)
	if(i=0) break
	qmKeyCode+i
	sel k
		case VK_CONTROL mod|MOD_CONTROL
		case VK_SHIFT mod|MOD_SHIFT
		case VK_MENU mod|MOD_ALT
		case VK_LWIN mod|MOD_WIN
		case else vk=k

if(vk=0) end "no key"
ret Register(hWnd hkId mod vk)
