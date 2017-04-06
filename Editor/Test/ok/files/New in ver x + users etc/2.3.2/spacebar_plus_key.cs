int vk=wait(0.2 KF) ;;wait for any key down max 0.5s; eat; store virtual-key code in vk
err ret ;;timeout
ifk-(V) goto retype ;;if spacebar not pressed, retype

sel vk
	case 'B' ;;key B
	key B CSL ;;erase space and press Ctrl+Shift+Left
	
	case 'N' ;;key N
	key B CSR ;;erase space and press Ctrl+Shift+Right
	
	case else ;;other keys
	 retype
	key (vk)
	ret


#if 0

 one two three four five
 one two four
 one two three four five
