 /remap_keyboard
function` KBDLLHOOKSTRUCT&h

 Edit this function to create your remapping.

 h - contains user-pressed key info. Don't change it.

 If you want to remap this key, let this function return the remapping:
   To remap to a key, return the virtual-key code.
   To remap to a string (eg a character), return the string.

 You can also edit this function while remap_keyboard thread is running. To apply, click Save or Compile button. On syntax error remap_keyboard thread will end. You can run this function (click Run button) to launch remap_keyboard.


int up=h.flags&LLKHF_UP ;;0 down, 1 up

  debug. Shows pressed keys, their virtual-key codes and scancodes. Disable when not needed.
FormatKeyString h.vkCode 0 &_s
out "%s %s, vk=0x%X, sc=0x%X%s" _s iif(up "up  " "down") h.vkCode h.scanCode iif(h.flags&LLKHF_EXTENDED ", extended" "")


 Examples:

 remap using scancodes; scancodes represent physical keys
sel h.scanCode
	case 0x5C ret VK_DELETE ;;key WinR to Delete
	case 0xD ret "''" ;;key += to character "
	case 0x28 ret 0x5B ;; to WinL'z
	 ...

  remap using virtual-key codes; virtual-key codes represent logical keys on current keyboard layout; look for the table in QM Help
sel h.vkCode
	case 'Q' ret 'W' ;;key Q to W
	case 'W' ret 'Q' ;;key W to Q
	 ...

#ret ;;everything you'll type below will not be included in the macro. Eg you can type any text here when testing.
