keybd_event VK_SHIFT 0 0 0
 keybd_event VK_LSHIFT 0 0 0
 keybd_event VK_RSHIFT 0 0 0

out "S%i L%i R%i" GetKeyState(VK_SHIFT)&0x8000!=0 GetKeyState(VK_LSHIFT)&0x8000!=0 GetKeyState(VK_RSHIFT)&0x8000!=0

keybd_event VK_SHIFT 0 2 0
keybd_event VK_LSHIFT 0 2 0
keybd_event VK_RSHIFT 0 2 0
